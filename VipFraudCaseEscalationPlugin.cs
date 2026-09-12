using System;
using Microsoft.Xrm.Sdk;

namespace BankCustomerService.Plugins
{
    public class VipFraudCaseEscalationPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(
                    typeof(IPluginExecutionContext));

            try
            {
                if (!context.InputParameters.Contains("Target"))
                    return;

                if (!(context.InputParameters["Target"] is Entity target))
                    return;

                if (target.LogicalName != "incident")
                    return;

                tracingService.Trace("VIP/Fraud Case Escalation Plugin started.");

                Entity preImage = null;

                if (context.PreEntityImages.Contains("PreImage"))
                {
                    preImage = context.PreEntityImages["PreImage"];
                }

                bool shouldEscalate = false;

                // PRIORITY
                OptionSetValue priority = null;

                if (target.Contains("prioritycode"))
                {
                    priority = target.GetAttributeValue<OptionSetValue>("prioritycode");
                }
                else if (preImage != null && preImage.Contains("prioritycode"))
                {
                    priority = preImage.GetAttributeValue<OptionSetValue>("prioritycode");
                }

                if (priority != null && priority.Value == 1)
                {
                    shouldEscalate = true;
                    tracingService.Trace("High priority case detected.");
                }

                // CASE CATEGORY
                OptionSetValue caseCategory = null;

                if (target.Contains("cr805_casecategory"))
                {
                    caseCategory =
                        target.GetAttributeValue<OptionSetValue>("cr805_casecategory");
                }
                else if (preImage != null && preImage.Contains("cr805_casecategory"))
                {
                    caseCategory =
                        preImage.GetAttributeValue<OptionSetValue>("cr805_casecategory");
                }

                if (caseCategory != null &&
                    caseCategory.Value == 576030002)
                {
                    shouldEscalate = true;
                    tracingService.Trace("Fraud case detected.");
                }

                // CUSTOMER TIER
                OptionSetValue customerTier = null;

                if (target.Contains("cr805_customertier"))
                {
                    customerTier =
                        target.GetAttributeValue<OptionSetValue>("cr805_customertier");
                }
                else if (preImage != null && preImage.Contains("cr805_customertier"))
                {
                    customerTier =
                        preImage.GetAttributeValue<OptionSetValue>("cr805_customertier");
                }

                if (customerTier != null &&
                    customerTier.Value == 576030002)
                {
                    shouldEscalate = true;
                    tracingService.Trace("VIP customer detected.");
                }

                if (shouldEscalate)
                {
                    target["prioritycode"] = new OptionSetValue(1);
                    target["cr805_escalationrequired"] = true;

                    tracingService.Trace(
                        "Case escalated. Priority = High, Escalation Required = Yes.");
                }

                tracingService.Trace("Plugin completed.");
            }
            catch (Exception ex)
            {
                tracingService.Trace(
                    "Plugin Error: {0}",
                    ex.ToString());

                throw new InvalidPluginExecutionException(
                    "An error occurred in VIP/Fraud Case Escalation Plugin.",
                    ex);
            }
        }
    }
}