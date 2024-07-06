﻿using System.Net;

namespace FormAPI.Errors;

public class RestException : Exception
{
	public RestException(HttpStatusCode code, object? errors = null)
	{
		Code = code;
		Errors = errors;
	}

	public object? Errors { get; set; }

	public HttpStatusCode Code { get; }
}
