#pragma once
#include "WpdObject.h"

namespace WpdInterop
{
    ref class WpdFileCreationProxy;

    public ref class WpdFilesystemItem : WpdObject
    {
    public:
        WpdFilesystemItem(LP id, System::String^ name, bool IsFolder, IPortableDeviceContent* content, IPortableDeviceCapabilities *capabilities);
        virtual ~WpdFilesystemItem();
        const System::String^ Name;
        const bool IsFolder;
        array<WpdFilesystemItem^>^ GetChildren();
        void Delete();
        WpdFilesystemItem ^ CreateChildFolder(System::String^ name);
        WpdFileCreationProxy ^ CreateChildFile(System::String^ name, System::UInt64 size);
    private:
        IPortableDeviceContent* _content;
        IPortableDeviceCapabilities *_capabilities;
        void AssertCanCreateChildren();
        IPortableDeviceValues * InitializeValuesForContent(GUID contentType, System::String^ fileOrDirectoryName, UINT64 size);
    };
}