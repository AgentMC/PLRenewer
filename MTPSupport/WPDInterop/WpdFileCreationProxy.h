#pragma once
#include "WpdFilesystemItem.h"
namespace WpdInterop
{
    public ref class WpdFileCreationProxy
    {
    internal:
        WpdFileCreationProxy(IPortableDeviceDataStream * pdds, System::String^ itemName, IPortableDeviceContent* content, IPortableDeviceCapabilities * capabilities);
        virtual ~WpdFileCreationProxy();
    public:
        void WriteContent(array<System::Byte>^ data, System::UInt32 length);
        WpdFilesystemItem^ CommitAndDispose();
        UINT64 GetLength();
        UINT64 GetPosition();
        void SetPosition(UINT64 pos);
    private:
        IPortableDeviceDataStream * _pdds;
        IPortableDeviceContent* _content;
        IPortableDeviceCapabilities *_capabilities;
        System::String^ _wpdItemName;
        WpdFilesystemItem^ CommitAndDisposeInternal(bool generateWpdItem);
    };
}