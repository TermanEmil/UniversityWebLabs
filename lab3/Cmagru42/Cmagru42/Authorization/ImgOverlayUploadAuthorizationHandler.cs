using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Presentation.Authorization
{
    public class ImgOverlayUploadAuthorizationHandler
		: AuthorizationHandler<OperationAuthorizationRequirement>
    {      
		protected override Task HandleRequirementAsync(
			AuthorizationHandlerContext context,
			OperationAuthorizationRequirement requirement)
		{
			if (context.User == null)
				return Task.CompletedTask;

			if (context.User.IsInRole(Constants.ImgOverlayerRole))
				context.Succeed(requirement);
            
			return Task.CompletedTask;
		}
	}
}
