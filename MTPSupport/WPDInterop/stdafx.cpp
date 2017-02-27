// stdafx.cpp : source file that includes just the standard includes
// WPDInterop.pch will be the pre-compiled header
// stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"

int main(array<System::String^>^ args){

}

System::Tuple<bool, System::String^, UINT64, UINT64>^ GetObjectDetails(IPortableDeviceContent * content, LP objectId)
{
    IPortableDeviceProperties * props;
    auto hr = content->Properties(&props);
    if (FAILED(hr)) throw gcnew System::Exception("! Failed to get Properties, hr =" + hr);

    IPortableDeviceKeyCollection * keys;
    hr = CoCreateInstance(CLSID_PortableDeviceKeyCollection, nullptr, CLSCTX::CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&keys));
    if (FAILED(hr))throw gcnew System::Exception("! Failed to create PortableDeviceKeyCollection, hr =" + hr);

    keys->Add(WPD_OBJECT_NAME);
    keys->Add(WPD_OBJECT_ORIGINAL_FILE_NAME);
    keys->Add(WPD_OBJECT_CONTENT_TYPE);
    keys->Add(WPD_STORAGE_FREE_SPACE_IN_BYTES);
    keys->Add(WPD_STORAGE_CAPACITY);

    IPortableDeviceValues * values;
    hr = props->GetValues(objectId, keys, &values);
    if (FAILED(hr))throw gcnew System::Exception("! Failed to get Values, hr = " + hr);

    LP string;
    GUID guid;
    UINT64 free = 0, capacity = 0;
    hr = values->GetStringValue(WPD_OBJECT_ORIGINAL_FILE_NAME, &string);
    if (FAILED(hr))
    {
        values->GetStringValue(WPD_OBJECT_NAME, &string);
    }
    values->GetGuidValue(WPD_OBJECT_CONTENT_TYPE, &guid);
    values->GetUnsignedLargeIntegerValue(WPD_STORAGE_FREE_SPACE_IN_BYTES, &free);
    values->GetUnsignedLargeIntegerValue(WPD_STORAGE_CAPACITY, &capacity);

    auto result = System::Tuple::Create(guid == WPD_CONTENT_TYPE_FOLDER, gcnew System::String(string), free, capacity);

    CoTaskMemFree(string);
    values->Release();
    keys->Release();
    props->Release();
    
    return result;
}

IPortableDeviceValues* CreateIPortableDeviceValues()
{
    IPortableDeviceValues *values;
    auto hr = CoCreateInstance(CLSID_PortableDeviceValues, nullptr, CLSCTX::CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&values));
    if (FAILED(hr)) throw gcnew System::Exception("! Failed to create PortableDeviceValues, hr =" + hr);
    return values;
}

LP SystemStringToLp(System::String^ s)
{
    auto ptr = PtrToStringChars(s);
    void* ptrptr = (void*)(&ptr);
    return *(LP*)ptrptr;
}