#include "stdafx.h"
#include "WpdFilesystemItem.h"
#include "WpdFileCreationProxy.h"


WpdInterop::WpdFilesystemItem::WpdFilesystemItem(LP id, System::String^ name, bool isFolder, IPortableDeviceContent* content, IPortableDeviceCapabilities *capabilities)
    :WpdObject(id), IsFolder(isFolder), _content(content), _capabilities(capabilities)
{
    Name = name;
    _content->AddRef();
    _capabilities->AddRef();
}

WpdInterop::WpdFilesystemItem::~WpdFilesystemItem()
{
    _content->Release();
    _capabilities->Release();
}

array<WpdInterop::WpdFilesystemItem^>^ WpdInterop::WpdFilesystemItem::GetChildren()
{
    auto children = gcnew System::Collections::Generic::List<WpdFilesystemItem^>();
    IEnumPortableDeviceObjectIDs * enumObj;
    auto hr = _content->EnumObjects(0, _id, nullptr, &enumObj);
    if (FAILED(hr)) throw gcnew System::Exception("!Failed to Retrieve Enum pointer, hr = " + hr);

    const DWORD batch = 100;
    DWORD fetched;
    LP buffer[batch];
    do
    {
        hr = enumObj->Next(batch, buffer, &fetched);
        for (size_t j = 0; j < fetched; j++)
        {
            auto objid = buffer[j];
            auto props = GetObjectDetails(_content, objid);
            children->Add(gcnew WpdFilesystemItem(objid, props->Item2, props->Item1, _content, _capabilities));
            CoTaskMemFree(buffer[j]);
        }
    } while (hr == S_OK);

    enumObj->Release();
    return children->ToArray();
}

void WpdInterop::WpdFilesystemItem::Delete()
{
    tagDELETE_OBJECT_OPTIONS recursionOption = PORTABLE_DEVICE_DELETE_NO_RECURSION;

    if (IsFolder){
        auto children = GetChildren();
        if (children->Length > 0){
            IPortableDeviceValues *options;
            if (SUCCEEDED(_capabilities->GetCommandOptions(WPD_COMMAND_OBJECT_MANAGEMENT_DELETE_OBJECTS, &options)))
            {
                BOOL supported;
                if (SUCCEEDED(options->GetBoolValue(WPD_OPTION_OBJECT_MANAGEMENT_RECURSIVE_DELETE_SUPPORTED, &supported)))
                {
                    if (supported) recursionOption = PORTABLE_DEVICE_DELETE_WITH_RECURSION;
                }
                options->Release();
            }
            for each (auto child in children)
            {
                if (recursionOption == PORTABLE_DEVICE_DELETE_NO_RECURSION) child->Delete();
                delete child;
            }
        }
    }

    IPortableDevicePropVariantCollection *ids, *errors;
    auto hr = CoCreateInstance(CLSID_PortableDevicePropVariantCollection, nullptr, CLSCTX::CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&ids));
    if (FAILED(hr)) throw gcnew System::Exception("! Failed to create PortableDevicePropVariantCollection, hr =" + hr);

    PROPVARIANT id;
    PropVariantInit(&id);
    id.vt = VT_LPWSTR;
    id.pwszVal = _id;

    ids->Add(&id);

    hr = _content->Delete(recursionOption, ids, &errors);
    if (FAILED(hr)) {
        if (hr == S_FALSE){//At least one object could not be deleted. The ppResults parameter contains the per-object error code.
            hr = errors->GetAt(0, &id);
            if (SUCCEEDED(hr)) hr = id.lVal;
        }
        throw gcnew System::Exception("! Failed to delete object, hr = " + hr);
    }

    if (errors) errors->Release();
    ids->Release();
}

WpdInterop::WpdFilesystemItem ^ WpdInterop::WpdFilesystemItem::CreateChildFolder(System::String^ name)
{
    AssertCanCreateChildren();

    LP newFolderId;

    auto options = InitializeValuesForContent(WPD_CONTENT_TYPE_FOLDER, name, 0);

    auto hr = _content->CreateObjectWithPropertiesOnly(options, &newFolderId);
    if (FAILED(hr)) throw gcnew System::Exception("! Failed to create folder, hr =" + hr);

    auto folder = gcnew WpdInterop::WpdFilesystemItem(newFolderId, name, true, _content, _capabilities);
    
    CoTaskMemFree(newFolderId);
    options->Release();    

    return folder;
}

void WpdInterop::WpdFilesystemItem::AssertCanCreateChildren()
{
    if (!IsFolder) throw gcnew System::InvalidOperationException("Child items cannot be created on non-container item.");
}

IPortableDeviceValues * WpdInterop::WpdFilesystemItem::InitializeValuesForContent(GUID contentType, System::String^ fileOrDirectoryName, UINT64 size)
{
    fileOrDirectoryName = fileOrDirectoryName->Trim('/', '\\');

    auto options = CreateIPortableDeviceValues();

    options->SetGuidValue(WPD_OBJECT_CONTENT_TYPE, contentType); 
    options->SetStringValue(WPD_OBJECT_PARENT_ID, _id);
    if (contentType == WPD_CONTENT_TYPE_FOLDER)
    {
        options->SetStringValue(WPD_OBJECT_NAME, SystemStringToLp(fileOrDirectoryName));
    }
    else
    {
        options->SetStringValue(WPD_OBJECT_NAME, SystemStringToLp(System::IO::Path::GetFileNameWithoutExtension(fileOrDirectoryName)));
        options->SetStringValue(WPD_OBJECT_ORIGINAL_FILE_NAME, SystemStringToLp(fileOrDirectoryName));
        options->SetUnsignedLargeIntegerValue(WPD_OBJECT_SIZE, size);
        //todo: content format?
    }

    return options;
}

WpdInterop::WpdFileCreationProxy ^ WpdInterop::WpdFilesystemItem::CreateChildFile(System::String^ name, System::UInt64 size)
{
    GUID contentType = WPD_CONTENT_TYPE_GENERIC_FILE;
    auto lowerName = name->ToLower();
    if (lowerName->EndsWith(".mp3") ||
        lowerName->EndsWith(".wma") ||
        lowerName->EndsWith(".fla") ||
        lowerName->EndsWith(".flac")||
        lowerName->EndsWith(".ogg") ||
        lowerName->EndsWith(".m4a"))
    {
        contentType = WPD_CONTENT_TYPE_AUDIO;
    }
    auto options = InitializeValuesForContent(contentType, name, size);//todo: real content type for more filetypes
    
    IStream * writer;
    DWORD bufferSize = 0; //todo:use; currently unused!
    auto hr = _content->CreateObjectWithPropertiesAndData(options, &writer, &bufferSize, nullptr);
    if (FAILED(hr)) throw gcnew System::Exception("! Failed to get writer, hr =" + hr);

    options->Release();

    IPortableDeviceDataStream * pdds;
    hr = writer->QueryInterface(&pdds);
    if (FAILED(hr)) throw gcnew System::Exception("! Failed to query PortableDeviceDataStream, hr =" + hr);

    writer->Release();

    return gcnew WpdFileCreationProxy(pdds, name, _content, _capabilities);
}
