using System;
using System.Threading.Tasks;
using DataLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Presentation.Authorization
{
    public class ImgUploadAdministratorsAuthorizationHandler
		: AuthorizationHandler<OperationAuthorizationRequirement, ImgUpload>
    {
		protected override Task HandleRequirementAsync(
			AuthorizationHandlerContext context,
			OperationAuthorizationRequirement requirement,
			ImgUpload resource)
		{
			if (context.User == null)
				return Task.CompletedTask;

			if (context.User.IsInRole(Constants.AdminRole))
				context.Succeed(requirement);

			return Task.CompletedTask;
		}
	}
}
