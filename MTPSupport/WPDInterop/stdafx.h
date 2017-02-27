// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

// TODO: reference additional headers your program requires here
#include <Windows.h>
#include <vcclr.h>
#include <PortableDevice.h>
#include <PortableDeviceTypes.h>
#include <PortableDeviceApi.h>
#include <tchar.h>

System::Tuple<bool, System::String^, UINT64, UINT64>^ GetObjectDetails(IPortableDeviceContent * content, LP objectId);
IPortableDeviceValues* CreateIPortableDeviceValues();
LP SystemStringToLp(System::String^ s);

