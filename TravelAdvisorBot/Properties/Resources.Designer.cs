﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TravelAdvisorBot.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TravelAdvisorBot.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to I don&apos;t understand &apos;{0}&apos;. Please try again..
        /// </summary>
        internal static string RootDialog_SendWelcomeMessage_NotUnderstood {
            get {
                return ResourceManager.GetString("RootDialog_SendWelcomeMessage_NotUnderstood", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to I&apos;m sorry, but &apos;{0}&apos; isn&apos;t ready yet..
        /// </summary>
        internal static string RootDialog_SendWelcomeMessage_NotYetImplemented {
            get {
                return ResourceManager.GetString("RootDialog_SendWelcomeMessage_NotYetImplemented", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Search flights.
        /// </summary>
        internal static string RootDialog_SendWelcomeMessage_SearchFlights {
            get {
                return ResourceManager.GetString("RootDialog_SendWelcomeMessage_SearchFlights", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Search hotels.
        /// </summary>
        internal static string RootDialog_SendWelcomeMessage_SearchHotels {
            get {
                return ResourceManager.GetString("RootDialog_SendWelcomeMessage_SearchHotels", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to How can I help you today?.
        /// </summary>
        internal static string RootDialog_SendWelcomeMessage_Subtitle {
            get {
                return ResourceManager.GetString("RootDialog_SendWelcomeMessage_Subtitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Travel Advisor Bot.
        /// </summary>
        internal static string RootDialog_SendWelcomeMessage_Title {
            get {
                return ResourceManager.GetString("RootDialog_SendWelcomeMessage_Title", resourceCulture);
            }
        }
    }
}
