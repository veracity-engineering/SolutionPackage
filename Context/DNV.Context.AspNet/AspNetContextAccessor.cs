using System;
using System.Threading;
using DNV.Context.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DNV.Context.AspNet
{
    internal class AspNetContextAccessor<T>: IContextAccessor<T> where T: class
    {
	    private readonly AsyncLocal<AspNetContext<T>> _aspNetContext;
	    private readonly Func<HttpContext, (bool, T)> _ctxCreator;

	    public AspNetContextAccessor(Func<HttpContext, (bool, T)> ctxCreator)
	    {
		    _aspNetContext = new AsyncLocal<AspNetContext<T>>();
		    _ctxCreator = ctxCreator;
	    }

        public IAmbientContext<T>? Current => _aspNetContext.Value;

        internal void CreateContext(HttpContext httpContext)
        {
	        var (succeeded, context) = _ctxCreator(httpContext);
	        if (!succeeded)
		        return;
			_aspNetContext.Value = new AspNetContext<T>(context);
        }

        internal void CreateContext(T context)
        {
	        _aspNetContext.Value = new AspNetContext<T>(context);
        }
	}
}
