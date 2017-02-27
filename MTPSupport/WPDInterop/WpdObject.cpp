#include "stdafx.h"
#include "WpdObject.h"


WpdInterop::WpdObject::WpdObject(LP id) 
{
    auto length = _tcslen(id) + 1;
    
    _id = (LP)malloc(length*sizeof(TCHAR));
    _tcscpy_s(_id, length, id);
}

WpdInterop::WpdObject::~WpdObject() 
{
    free(_id);
}

System::String^ WpdInterop::WpdObject::GetId()
{
    return gcnew System::String(_id);
}