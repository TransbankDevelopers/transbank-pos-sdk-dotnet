//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.0
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace Transbank.POS.Utils {

public class BaseResponse : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal BaseResponse(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(BaseResponse obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~BaseResponse() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          TransbankWrapPINVOKE.delete_BaseResponse(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public int function {
    set {
      TransbankWrapPINVOKE.BaseResponse_function_set(swigCPtr, value);
    } 
    get {
      int ret = TransbankWrapPINVOKE.BaseResponse_function_get(swigCPtr);
      return ret;
    } 
  }

  public int responseCode {
    set {
      TransbankWrapPINVOKE.BaseResponse_responseCode_set(swigCPtr, value);
    } 
    get {
      int ret = TransbankWrapPINVOKE.BaseResponse_responseCode_get(swigCPtr);
      return ret;
    } 
  }

  public long commerceCode {
    set {
      TransbankWrapPINVOKE.BaseResponse_commerceCode_set(swigCPtr, value);
    } 
    get {
      long ret = TransbankWrapPINVOKE.BaseResponse_commerceCode_get(swigCPtr);
      return ret;
    } 
  }

  public string terminalId {
    set {
      TransbankWrapPINVOKE.BaseResponse_terminalId_set(swigCPtr, value);
    } 
    get {
      string ret = TransbankWrapPINVOKE.BaseResponse_terminalId_get(swigCPtr);
      return ret;
    } 
  }

  public int initilized {
    set {
      TransbankWrapPINVOKE.BaseResponse_initilized_set(swigCPtr, value);
    } 
    get {
      int ret = TransbankWrapPINVOKE.BaseResponse_initilized_get(swigCPtr);
      return ret;
    } 
  }

  public BaseResponse() : this(TransbankWrapPINVOKE.new_BaseResponse(), true) {
  }

}

}
