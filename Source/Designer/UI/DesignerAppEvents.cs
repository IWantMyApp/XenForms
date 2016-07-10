using System;
using Ninject;
using Xamarin.Forms;
using XenForms.Core.Designer;
using XenForms.Core.Designer.Generators;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Messages;
using XenForms.Core.Reflection;
using XenForms.Core.Widgets;
using XenForms.Designer.XamarinForms.UI.Reactions;

namespace XenForms.Designer.XamarinForms.UI
{
    public static class DesignerAppEvents
    {
        private static DescriptorRegistrar _dr;
        private static DescriptorRegistrar Dr
        {
            get
            {
                if (_dr == null)
                {
                    var tf = App.Kernel.Get<ITypeFinder>();
                    _dr = DescriptorRegistrar.Create(tf);
                }

                return _dr;
            }
        }


        public static void SetupTypeDescriptors()
        {
            Dr.Reset();

            Dr.Add(typeof(ValueType), new StaticGenerator());
            Dr.Add(typeof(Enum), new EnumGenerator());
            Dr.Add(typeof(ImageSource), XenPropertyDescriptors.Image);
        }


        public static void SetupDesignSurface(Page page)
        {
            XamarinFormsTypeConverters.EnhancedXamlConfiguration.Initialize();

            Reaction.Reset();
            Reaction.GetServices = obj => App.Kernel.Inject(obj);
            Reaction.Register<SupportedTypesRequest, SupportedTypesReaction>();

            XamarinFormsReaction.Register<GetVisualTreeRequest, GetVisualTreeReaction<VisualElement>>(page);
            XamarinFormsReaction.Register<CreateWidgetRequest, CreateWidgetReaction>(page);
            XamarinFormsReaction.Register<GetObjectRequest, GetObjectReaction<VisualElement>>(page);
            XamarinFormsReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction<VisualElement>>(page);
            XamarinFormsReaction.Register<EditCollectionRequest, EditCollectionReaction>(page);
            XamarinFormsReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            XamarinFormsReaction.Register<DeleteWidgetRequest, DeleteWidgetReaction>(page);
            XamarinFormsReaction.Register<OpenXamlRequest, OpenXamlReaction>(page);
            XamarinFormsReaction.Register<GetWidgetEventsRequest, GetWidgetEventsReaction>(page);
            XamarinFormsReaction.Register<LoadEventsRequest, LoadEventsReaction>(page);
            XamarinFormsReaction.Register<LoadProjectRequest, LoadProjectReaction>(page);
            XamarinFormsReaction.Register<CreateStackLayoutRequest, CreateStackLayoutReaction>(page);
            XamarinFormsReaction.Register<CreateGridRequest, CreateGridReaction>(page);
            XamarinFormsReaction.Register<GetConstructorsRequest, GetConstructorsReaction<VisualElement>>(page);
            XamarinFormsReaction.Register<CallConstructorRequest, CallConstructorReaction>(page);
            XamarinFormsReaction.Register<GetAttachedPropertiesRequest, GetAttachedPropertiesReaction>(page);
            XamarinFormsReaction.Register<SaveXamlRequest, SaveXamlReaction>(page);
            XamarinFormsReaction.Register<GetDesignSurfaceXamlRequest, GetDesignSurfaceXamlReaction>(page);
            XamarinFormsReaction.Register<NewPageRequest, NewPageReaction>(page);
            XamarinFormsReaction.Register<AddSupportedTypeRequest, AddSupportedTypesReaction>(page);
        }
    }
}