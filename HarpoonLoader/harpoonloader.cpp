#include <windows.h>
#include <string>
#include <assert.h>
#import "..\Build\Harpoon.Core.tlb" no_namespace

#define __DEBUG_MODE__

BOOL WINAPI DllMain(HINSTANCE, DWORD fdwReason, LPVOID) 
{
	switch (fdwReason) 
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}

	return TRUE;

}

extern "C" __declspec(dllexport) DWORD initialize(LPVOID param)
{
#ifdef __DEBUG_MODE__
	AllocConsole();
	SetConsoleTitleA("Harpoon debug console");
#endif

	std::string error = "";
	DWORD errorId = S_OK;
	LPSTR messageBuffer = nullptr;

	IInitializablePtr ptr;

#ifdef __DEBUG_MODE__
	FILE* fp;
	freopen_s(&fp, "CONOUT$", "w", stdout);
	printf("Loading HarpoonCore\n");
#endif

	HRESULT res;

	if (FAILED(res = CoInitialize(NULL))) {
#ifdef __DEBUG_MODE__
		printf("Couldn't initialize HarpoonLoader\n");
#endif
		goto failed;
	}

	CLSID clsid;

	if (FAILED(res = CLSIDFromProgID(OLESTR("HarpoonCore"), &clsid))) {
#ifdef __DEBUG_MODE__
		printf("Couldn't get HarpoonCore\n");
#endif
		goto failed;
	}

	if (FAILED(res = ptr.CreateInstance(clsid))) {
#ifdef __DEBUG_MODE__
		printf("Couldn't instantiate HarpoonCore\n");
#endif
		goto failed;
	}
	printf("initializing Harpoon.Core");

	if (FAILED(res = ptr->Initialize()))
	{
#ifdef __DEBUG_MODE__
		printf("fuck life and everything around it\n");
#endif
		goto failed;
	}
	goto succeeded;

failed:

	error = std::string(messageBuffer, FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS, NULL, res, MAKELANGID(LANG_ENGLISH, SUBLANG_DEFAULT), (LPSTR)&messageBuffer, 0, NULL));

#ifdef __DEBUG_MODE__
	printf("%s\n", error.c_str());
	fclose(fp);
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