#pragma once
#include "WpdFilesystemItem.h"
namespace WpdInterop
{
    public ref class WpdDriveInfo:WpdFilesystemItem
    {
    public:
        WpdDriveInfo(LP id, System::String^ name, bool isFolder, UINT64 freeSpace, UINT64 totalCapacity, IPortableDeviceContent* content, IPortableDeviceCapabilities *capabilities);
        virtual ~WpdDriveInfo();
        UINT64 FreeSpace, Capacity;
    };
}