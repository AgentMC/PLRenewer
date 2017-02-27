#include "stdafx.h"
#include "WpdDriveInfo.h"


WpdInterop::WpdDriveInfo::WpdDriveInfo(LP id, System::String^ name, bool isFolder, UINT64 freeSpace, UINT64 totalCapacity, IPortableDeviceContent* content, IPortableDeviceCapabilities *capabilities)
    :WpdFilesystemItem(id, name, isFolder, content, capabilities)
{
    FreeSpace = freeSpace;
    Capacity = totalCapacity;
}


WpdInterop::WpdDriveInfo::~WpdDriveInfo()
{
}
