#include <windows.h>
#include <string>
#include <assert.h>
#include <functional>
#include <vector>

#define __DEBUG_MODE__
#define __namespace__ "Harpoon.Core"
#define __class__ "HarpoonCore"
#define __function__ "Initialize"

std::vector<std::pair<bool(*)(), const char*>> funcsToInit;

bool addFuncToInit(bool (*f)(), const char *str) {
	funcsToInit.push_back(std::pair<bool(*)(), const char*>(f, str));
	return true;
}

#define MONO_FUNC_T(x, params, y, z) 												\
x (*y)(params) = nullptr;															\
bool y##_func(){ y = (x (*)(params)) GetProcAddress(z, #y); return y != nullptr; }	\
const bool y##_funcInitb = addFuncToInit(y##_func, #y);

#define MONO_FUNC(params, y, z) MONO_FUNC_T(PVOID, params, y, z);

#define _(...) __VA_ARGS__

HMODULE mono;

MONO_FUNC(_(), mono_domain_get, mono);
MONO_FUNC(_(PVOID), mono_thread_attach, mono);
MONO_FUNC(_(), mono_get_root_domain, mono);
MONO_FUNC(_(PVOID, PCHAR), mono_domain_assembly_open, mono);
MONO_FUNC(_(PVOID, PCHAR, PCHAR), mono_class_from_name, mono);
MONO_FUNC(_(PVOID, PCHAR, DWORD), mono_class_get_method_from_name, mono);
MONO_FUNC(_(PVOID, PVOID, PVOID*, PVOID), mono_runtime_invoke, mono);
MONO_FUNC(_(PVOID), mono_assembly_get_image, mono);

BOOL WINAPI DllMain(HINSTANCE, DWORD fdwReason, LPVOID)  {

	switch (fdwReason)  {
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}

	return TRUE;

}

int mainFunc() {
	return 1;
}

extern "C" __declspec(dllexport) DWORD initialize(LPVOID param) {

	//Initialize console

#ifdef __DEBUG_MODE__
	AllocConsole();
	SetConsoleTitleA("Harpoon debug console");
#endif

	std::string error = "";
	DWORD errorId = S_OK;
	LPSTR messageBuffer = nullptr;
	

#ifdef __DEBUG_MODE__
	FILE* fp;
	freopen_s(&fp, "CONOUT$", "w", stdout);
	printf("Loading HarpoonCore\n");
#endif

	//Intialize mono and its functions

	mono = LoadLibraryA("mono.dll");

	HRESULT res = S_OK;

	if (mono == NULL) {
#ifdef __DEBUG_MODE__
		printf("Couldn't initialize mono\n");
#endif
		goto failed;
	}
	
	for(auto f : funcsToInit) {

		if (!f.first()) {
#ifdef __DEBUG_MODE__
			printf("Couldn't initialize mono function (%s)\n", f.second);
#endif
			goto failed;
		}
	}

	//Run mono on Harpoon.Core

	PVOID rootDomain = mono_get_root_domain();

	if (rootDomain == NULL) {
#ifdef __DEBUG_MODE__
		printf("Couldn't initialize mono root domain\n");
#endif
		goto failed;
	}

	mono_thread_attach(rootDomain);

	PVOID monoDomain = mono_domain_get();

	if (monoDomain == NULL) {
#ifdef __DEBUG_MODE__
		printf("Couldn't initialize mono domain\n");
#endif
		goto failed;
	}

	PVOID domainAssembly = mono_domain_assembly_open(monoDomain, "Harpoon.Core.dll");

	if (domainAssembly == NULL) {
#ifdef __DEBUG_MODE__
		printf("Couldn't initialize mono domain assembly\n");
#endif
		goto failed;
	}

	PVOID image = mono_assembly_get_image(domainAssembly);

	if (image == NULL) {
#ifdef __DEBUG_MODE__
		printf("Couldn't initialize mono image\n");
#endif
		goto failed;
	}

	PVOID monoClass = mono_class_from_name(image, __namespace__, __class__);

	if (monoClass == NULL) {
#ifdef __DEBUG_MODE__
		printf("Couldn't initialize mono class (%s.%s)\n", __namespace__, __class__);
#endif
		goto failed;
	}

	PVOID monoMethod = mono_class_get_method_from_name(monoClass, __function__, 0);

	if (monoMethod == NULL) {
#ifdef __DEBUG_MODE__
		printf("Couldn't initialize mono method (%s.%s.%s)\n", __namespace__, __class__, __function__);
#endif
		goto failed;
	}

	//Invoke method
	mono_runtime_invoke(monoMethod, NULL, NULL, NULL);

	goto succeeded;

failed:

	error = std::string(messageBuffer, FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS, NULL, res, MAKELANGID(LANG_ENGLISH, SUBLANG_DEFAULT), (LPSTR)&messageBuffer, 0, NULL));

#ifdef __DEBUG_MODE__
	printf("%s\n", error.c_str());
	fclose(fp);

	Sleep(10000);

	FreeConsole();
#endif

	LocalFree(messageBuffer);

	CoUninitialize();
	return 0;

succeeded:

#ifdef __DEBUG_MODE__
	printf("Successfully created C# instance");
	fclose(fp);

	FreeConsole();
#endif

	CoUninitialize();
	return 1;
}