//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.12
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace Transbank.POS.Utils {

public class LoadKeyCloseResponse : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal LoadKeyCloseResponse(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(LoadKeyCloseResponse obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~LoadKeyCloseResponse() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          TransbankWrapPINVOKE.delete_LoadKeyCloseResponse(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public int function {
    set {
      TransbankWrapPINVOKE.LoadKeyCloseResponse_function_set(swigCPtr, value);
    } 
    get {
      int ret = TransbankWrapPINVOKE.LoadKeyCloseResponse_function_get(swigCPtr);
      return ret;
    } 
  }

  public int responseCode {
    set {
      TransbankWrapPINVOKE.LoadKeyCloseResponse_responseCode_set(swigCPtr, value);
    } 
    get {
      int ret = TransbankWrapPINVOKE.LoadKeyCloseResponse_responseCode_get(swigCPtr);
      return ret;
    } 
  }

  public long commerceCode {
    set {
      TransbankWrapPINVOKE.LoadKeyCloseResponse_commerceCode_set(swigCPtr, value);
    } 
    get {
      long ret = TransbankWrapPINVOKE.LoadKeyCloseResponse_commerceCode_get(swigCPtr);
      return ret;
    } 
  }

  public int terminalId {
    set {
      TransbankWrapPINVOKE.LoadKeyCloseResponse_terminalId_set(swigCPtr, value);
    } 
    get {
      int ret = TransbankWrapPINVOKE.LoadKeyCloseResponse_terminalId_get(swigCPtr);
      return ret;
    } 
  }

  public int initilized {
    set {
      TransbankWrapPINVOKE.LoadKeyCloseResponse_initilized_set(swigCPtr, value);
    } 
    get {
      int ret = TransbankWrapPINVOKE.LoadKeyCloseResponse_initilized_get(swigCPtr);
      return ret;
    } 
  }

  public LoadKeyCloseResponse() : this(TransbankWrapPINVOKE.new_LoadKeyCloseResponse(), true) {
  }

}

}
