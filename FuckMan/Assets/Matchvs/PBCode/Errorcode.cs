// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: errorcode.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Matchvs {

  /// <summary>Holder for reflection information generated from errorcode.proto</summary>
  public static partial class ErrorcodeReflection {

    #region Descriptor
    /// <summary>File descriptor for errorcode.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ErrorcodeReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg9lcnJvcmNvZGUucHJvdG8SBnN0cmVhbSryAQoJRXJyb3JDb2RlEgsKB05v",
            "RXJyb3IQABIHCgJPSxDIARINCghBY2NlcHRlZBDKARIOCglOb0NvbnRlbnQQ",
            "zAESDwoKQmFkUmVxdWVzdBCQAxIRCgxVbmF1dGhvcml6ZWQQkQMSFAoPU2ln",
            "bmF0dXJlRmFpbGVkEJIDEg4KCUZvcmJpZGRlbhCTAxINCghOb3RGb3VuZBCU",
            "AxIYChNJbnRlcm5hbFNlcnZlckVycm9yEPQDEhMKDk5vdEltcGxlbWVudGVk",
            "EPUDEg8KCkJhZEdhdGV3YXkQ9gMSFwoSU2VydmljZVVuYXZhaWxhYmxlEPcD",
            "QgqqAgdNYXRjaHZzYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Matchvs.ErrorCode), }, null));
    }
    #endregion

  }
  #region Enums
  public enum ErrorCode {
    [pbr::OriginalName("NoError")] NoError = 0,
    [pbr::OriginalName("OK")] Ok = 200,
    [pbr::OriginalName("Accepted")] Accepted = 202,
    [pbr::OriginalName("NoContent")] NoContent = 204,
    [pbr::OriginalName("BadRequest")] BadRequest = 400,
    [pbr::OriginalName("Unauthorized")] Unauthorized = 401,
    [pbr::OriginalName("SignatureFailed")] SignatureFailed = 402,
    [pbr::OriginalName("Forbidden")] Forbidden = 403,
    [pbr::OriginalName("NotFound")] NotFound = 404,
    [pbr::OriginalName("InternalServerError")] InternalServerError = 500,
    [pbr::OriginalName("NotImplemented")] NotImplemented = 501,
    [pbr::OriginalName("BadGateway")] BadGateway = 502,
    [pbr::OriginalName("ServiceUnavailable")] ServiceUnavailable = 503,
  }

  #endregion

}

#endregion Designer generated code
