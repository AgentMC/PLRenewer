#include "stdafx.h"
#include "Manager.h"


WpdInterop::Manager::Manager()
{
    _manager = nullptr;
    IPortableDeviceManager* proxy;
    auto hr = CoCreateInstance(CLSID_PortableDeviceManager, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&proxy));
    if (FAILED(hr))throw gcnew System::Exception("Failed to create CLSID_PortableDeviceManager, hr = "+ hr);
    _manager = proxy;
}

WpdInterop::Manager::~Manager()
{
    if (_manager != nullptr) _manager->Release();
}

array<WpdInterop::Device^>^ WpdInterop::Manager::GetDevices()
{
    auto devices = gcnew System::Collections::Generic::List<Device^>();
    auto hr = _manager->RefreshDeviceList();
    if (FAILED(hr)) throw gcnew System::Exception("Cannot refresh device list");
    DWORD data;
    hr = _manager->GetDevices(nullptr, &data);
    if (FAILED(hr)) throw gcnew System::Exception("Call GetDevices() for count failed, hr=" + hr);
    auto namez = new LP[data];
    hr = _manager->GetDevices(namez, &data);
    if (FAILED(hr)) throw gcnew System::Exception("Call GetDevices() for real data failed, hr=" + hr);
    DWORD k = 1024, kDefault = k;
    TCHAR buffer[1024];
    for (size_t i = 0; i < data; i++)
    {
        _manager->GetDeviceManufacturer(namez[i], buffer, &k);
        auto manufacturer = gcnew System::String(buffer);
        k = kDefault;
        _manager->GetDeviceFriendlyName(namez[i], buffer, &k);
        auto name = gcnew System::String(buffer);
        k = kDefault;
        _manager->GetDeviceDescription(namez[i], buffer, &k);
        auto description = gcnew System::String(buffer);
        k = kDefault;
        devices->Add(gcnew Device(namez[i], name, manufacturer, description));
    }
    delete[] namez;
    return devices->ToArray();
}