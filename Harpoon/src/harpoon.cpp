#include "harpoon.h"
#include <locale>
#include <codecvt>
#include <assert.h>
#include <process.h>
#include <fstream>
using namespace hp;

void check(void *func, char *str) {

	if (func == nullptr) {
		printf("%s\n", str);
		assert(false && "Error thrown");
	}
}

void Harpoon::penetrate(DWORD processId, std::string dllPath) {

	//Get process
	HANDLE process = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_CREATE_THREAD | PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, FALSE, processId);
	check(process, "Couldn't open process");

	//Copy the string into the other process
	DWORD size = (DWORD) dllPath.size() + 1;
	LPVOID remote = VirtualAllocEx(process, NULL, size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
	check(remote, "Couldn't allocate space into process");
	check((void*)WriteProcessMemory(process, remote, (PVOID) dllPath.c_str(), size, NULL), "Couldn't write string into memory");

	//Load the dll into memory
	LPTHREAD_START_ROUTINE loadLibraryProc = (LPTHREAD_START_ROUTINE) GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
	check(loadLibraryProc, "Couldn't get 'load library'");

	//Run the load library function
	HANDLE thread = CreateRemoteThread(process, NULL, 0, loadLibraryProc, remote, 0, NULL);
	check(thread, "Couldn't start library on remote process");

	//Wait for it to finish and stop
	WaitForSingleObject(thread, INFINITE);

	VirtualFreeEx(process, remote, 0, MEM_RELEASE);
	CloseHandle(thread);
	CloseHandle(process);

}

int help(std::string error, bool showDefault = true) {

	printf("%s\n", error.c_str());

	if (showDefault) {

		printf("Commands:\n");
		printf("-penetrate <pId> <dllPath>\nPenetrates Harpoon into an exe, to hook it so you can execute code.\n");

	}

	return 0;
}

int main(int argc, char *argv[]) {

	if (argc < 2)
		return help("Not enough arguments");

	std::string currentProcessName = argv[0];

	std::string cwd = currentProcessName.substr(0, currentProcessName.find_last_of('\\'));

	std::string arg = argv[1];

	if (arg == "-penetrate") {

		if (argc < 4) return help("Syntax: -penetrate <pId> <dllPath>", false);

		std::string pid = argv[2];
		std::string dll = argv[3];

		dll = cwd + "\\" + dll;

		int id = std::stoi(pid);

		if (id == 0) return help("Syntax: -penetrate <pId> <dllPath>", false);

		Harpoon::penetrate((DWORD)id, dll);
		return 1;
	}

	return help("Couldn't find a matching argument (case sensitive)");
}