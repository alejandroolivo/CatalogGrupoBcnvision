﻿#pragma checksum "..\..\..\Forms\WPFMain.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "F2BC0BDDDF9870543352455600912790B073C2F5"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using Bcnvision_Catalog.Controls;
using Bcnvision_Catalog.Forms;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Bcnvision_Catalog.Forms {
    
    
    /// <summary>
    /// WPFMain
    /// </summary>
    public partial class WPFMain : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 38 "..\..\..\Forms\WPFMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border borderCentral;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\Forms\WPFMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid _gridLayoutMain;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\Forms\WPFMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image LogoPrincipal;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\..\Forms\WPFMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnProductos;
        
        #line default
        #line hidden
        
        
        #line 90 "..\..\..\Forms\WPFMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnAplicaciones;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\..\Forms\WPFMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnPresentaciones;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\Forms\WPFMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnInformes;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Bcnvision Catalog;component/forms/wpfmain.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\WPFMain.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.borderCentral = ((System.Windows.Controls.Border)(target));
            return;
            case 2:
            this._gridLayoutMain = ((System.Windows.Controls.Grid)(target));
            
            #line 39 "..\..\..\Forms\WPFMain.xaml"
            this._gridLayoutMain.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this._gridLayoutMain_MouseLeftButtonUp);
            
            #line default
            #line hidden
            return;
            case 3:
            this.LogoPrincipal = ((System.Windows.Controls.Image)(target));
            return;
            case 4:
            this.btnProductos = ((System.Windows.Controls.Button)(target));
            
            #line 80 "..\..\..\Forms\WPFMain.xaml"
            this.btnProductos.MouseEnter += new System.Windows.Input.MouseEventHandler(this.BtnProductos_MouseEnter);
            
            #line default
            #line hidden
            
            #line 80 "..\..\..\Forms\WPFMain.xaml"
            this.btnProductos.Click += new System.Windows.RoutedEventHandler(this.BtnProductos_Click);
            
            #line default
            #line hidden
            
            #line 80 "..\..\..\Forms\WPFMain.xaml"
            this.btnProductos.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.BtnProductos_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnAplicaciones = ((System.Windows.Controls.Button)(target));
            
            #line 90 "..\..\..\Forms\WPFMain.xaml"
            this.btnAplicaciones.MouseEnter += new System.Windows.Input.MouseEventHandler(this.BtnAplicaciones_MouseEnter);
            
            #line default
            #line hidden
            
            #line 90 "..\..\..\Forms\WPFMain.xaml"
            this.btnAplicaciones.Click += new System.Windows.RoutedEventHandler(this.BtnAplicaciones_Click);
            
            #line default
            #line hidden
            
            #line 90 "..\..\..\Forms\WPFMain.xaml"
            this.btnAplicaciones.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.BtnAplicaciones_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnPresentaciones = ((System.Windows.Controls.Button)(target));
            
            #line 100 "..\..\..\Forms\WPFMain.xaml"
            this.btnPresentaciones.MouseEnter += new System.Windows.Input.MouseEventHandler(this.BtnPresentaciones_MouseEnter);
            
            #line default
            #line hidden
            
            #line 100 "..\..\..\Forms\WPFMain.xaml"
            this.btnPresentaciones.Click += new System.Windows.RoutedEventHandler(this.BtnPresentaciones_Click);
            
            #line default
            #line hidden
            
            #line 100 "..\..\..\Forms\WPFMain.xaml"
            this.btnPresentaciones.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.BtnPresentaciones_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btnInformes = ((System.Windows.Controls.Button)(target));
            
            #line 110 "..\..\..\Forms\WPFMain.xaml"
            this.btnInformes.MouseEnter += new System.Windows.Input.MouseEventHandler(this.BtnInformes_MouseEnter);
            
            #line default
            #line hidden
            
            #line 110 "..\..\..\Forms\WPFMain.xaml"
            this.btnInformes.Click += new System.Windows.RoutedEventHandler(this.BtnInformes_Click);
            
            #line default
            #line hidden
            
            #line 110 "..\..\..\Forms\WPFMain.xaml"
            this.btnInformes.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.BtnInformes_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

