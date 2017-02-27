#pragma once
#include "WpdDriveInfo.h"
namespace WpdInterop
{
    public ref class Device : WpdObject
    {
    public:
        Device(LP id, System::String^ name, System::String^ manufacturer, System::String^ description);
        virtual ~Device();
        const System::String^ Name;
        const System::String^ Manufacturer;
        const System::String^ Description;
        array<WpdInterop::WpdDriveInfo^>^ GetStorage();
    private:
        IPortableDevice* _device;
        IPortableDeviceValues * _values;
        IPortableDeviceCapabilities * _capabilities;
        IPortableDeviceContent * _content;
        bool _opened;
    };
}