#pragma once

#include "Device.h"
namespace WpdInterop
{
    public ref class Manager
    {
    public:
        Manager();
        ~Manager();
        array<Device^>^ GetDevices();
    private:
        IPortableDeviceManager* _manager;
    };
}