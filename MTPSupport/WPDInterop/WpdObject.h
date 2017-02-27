#pragma once
namespace WpdInterop
{
    public ref class WpdObject abstract
    {
    public:
        System::String^ GetId();
    protected:
        WpdObject(LP id);
        ~WpdObject();
    internal:
        LP _id;
    };
}