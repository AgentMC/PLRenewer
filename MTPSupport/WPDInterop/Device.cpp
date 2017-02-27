#include "stdafx.h"
#include "Device.h"


WpdInterop::Device::Device(LP id, System::String^ name, System::String^ manufacturer, System::String^ description) 
    : WpdObject(id)
{
    Name = name;
    Manufacturer = manufacturer;
    Description = description;
    _device = nullptr;
    _values = nullptr;
    _capabilities = nullptr;
    _content = nullptr;

    IPortableDevice* proxy;
    auto hr = CoCreateInstance(CLSID_PortableDeviceFTM, nullptr, CLSCTX::CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&proxy));
    if (FAILED(hr)) throw gcnew System::Exception("! Failed to create PortableDevice, hr =" + hr);
    _device = proxy;

    _values = CreateIPortableDeviceValues();
    _values->SetStringValue(WPD_CLIENT_NAME, TEXT("PlRenewer"));
    _values->SetUnsignedIntegerValue(WPD_CLIENT_MAJOR_VERSION, 0);
    _values->SetUnsignedIntegerValue(WPD_CLIENT_MINOR_VERSION, 1);
    _values->SetUnsignedIntegerValue(WPD_CLIENT_REVISION, 1);
    _values->SetUnsignedIntegerValue(WPD_CLIENT_SECURITY_QUALITY_OF_SERVICE, SECURITY_IMPERSONATION);
    
    hr = _device->Open(id, _values);
    if (FAILED(hr))throw gcnew System::Exception("! Failed to open PortableDevice, hr =" + hr);
    _opened = true;

    IPortableDeviceCapabilities * proxy3;
    hr = _device->Capabilities(&proxy3);
    if (FAILED(hr))throw gcnew System::Exception("! Failed to retrieve Capabilities, hr =" + hr);
    _capabilities = proxy3;

    IPortableDeviceContent *proxy4;
    hr = _device->Content(&proxy4);
    if (FAILED(hr))throw gcnew System::Exception("! Failed to get Content interface, hr = " + hr);
    _content = proxy4;
}


WpdInterop::Device::~Device()
{
    if (_device != nullptr) 
    {
        if (_capabilities != nullptr) _capabilities->Release();
        if (_content != nullptr) _content->Release();
        if (_opened) _device->Close();
        _device->Release();
    }
    if (_values != nullptr) _values->Release();
}

array<WpdInterop::WpdDriveInfo^>^ WpdInterop::Device::GetStorage()
{
    auto drives = gcnew System::Collections::Generic::List<WpdDriveInfo^>();
    IPortableDevicePropVariantCollection * storage;
    auto hr = _capabilities->GetFunctionalObjects(WPD_FUNCTIONAL_CATEGORY_STORAGE, &storage);
    if (FAILED(hr))throw gcnew System::Exception("! Failed to get Functional objects, hr = " + hr);

    DWORD count;
    storage->GetCount(&count);
    for (size_t j = 0; j < count; j++)
    {
        PROPVARIANT pv;
        PropVariantInit(&pv);
        storage->GetAt(j, &pv);
        auto props = GetObjectDetails(_content, pv.pwszVal);
        drives->Add(gcnew WpdDriveInfo(pv.pwszVal, props->Item2, true, props->Item3, props->Item4, _content, _capabilities));
        PropVariantClear(&pv);
    }
    storage->Release();

    return drives->ToArray();
}
