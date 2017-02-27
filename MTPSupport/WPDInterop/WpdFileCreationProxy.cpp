#include "stdafx.h"
#include "WpdFileCreationProxy.h"

WpdInterop::WpdFileCreationProxy::WpdFileCreationProxy(IPortableDeviceDataStream * pdds, System::String^ itemName, IPortableDeviceContent* content, IPortableDeviceCapabilities * capabilities) 
    :_pdds(pdds), _content(content), _capabilities(capabilities)
{
    _wpdItemName = itemName;
    _pdds->AddRef();
    _content->AddRef();
    _capabilities->AddRef();
}

WpdInterop::WpdFileCreationProxy::~WpdFileCreationProxy() 
{
    CommitAndDisposeInternal(false);
}

void WpdInterop::WpdFileCreationProxy::WriteContent(array<System::Byte>^ data, System::UInt32 length)
{
    auto intptr = System::Runtime::InteropServices::Marshal::UnsafeAddrOfPinnedArrayElement(data, 0);
    DWORD written = 0;
    auto hr = _pdds->Write((void*)intptr, length, &written);
    if (FAILED(hr)) throw gcnew System::Exception("! Failed to write data, hr =" + hr);
}

WpdInterop::WpdFilesystemItem^ WpdInterop::WpdFileCreationProxy::CommitAndDispose()
{
    return CommitAndDisposeInternal(true);
}

WpdInterop::WpdFilesystemItem^ WpdInterop::WpdFileCreationProxy::CommitAndDisposeInternal(bool generateWpdItem)
{
    auto hr = _pdds->Commit(STGC::STGC_DEFAULT);
    if (FAILED(hr)) throw gcnew System::Exception("! Failed to commit, hr =" + hr);

    WpdFilesystemItem^ item = nullptr;
    if (generateWpdItem)
    {
        LP objId;
        hr = _pdds->GetObjectID(&objId);
        if (FAILED(hr)) throw gcnew System::Exception("! Failed to get ObjectId, hr =" + hr);

        item = gcnew WpdFilesystemItem(objId, _wpdItemName, false, _content, _capabilities);

        CoTaskMemFree(objId);
    }

    _pdds->Release();
    _content->Release();
    _capabilities->Release();

    return item;
}

UINT64 WpdInterop::WpdFileCreationProxy::GetLength()
{
    STATSTG stg;
    auto hr = _pdds->Stat(&stg, STATFLAG_NONAME);
    if (FAILED(hr)) throw gcnew System::Exception("! Failed to get STAT, hr =" + hr);

    return stg.cbSize.QuadPart;
}

UINT64 WpdInterop::WpdFileCreationProxy::GetPosition()
{
    ULARGE_INTEGER position;
    LARGE_INTEGER move;
    move.QuadPart = 0;
    auto hr = _pdds->Seek(move, STREAM_SEEK::STREAM_SEEK_CUR, &position);
    if (FAILED(hr)) throw gcnew System::Exception("! Failed to Seek for position, hr =" + hr);

    return position.QuadPart;
}

void WpdInterop::WpdFileCreationProxy::SetPosition(UINT64 pos)
{
    LARGE_INTEGER move;
    move.QuadPart = pos;
    auto hr = _pdds->Seek(move, STREAM_SEEK::STREAM_SEEK_SET, nullptr);
    if (FAILED(hr)) throw gcnew System::Exception("! Failed to Seek for position, hr =" + hr);
}