﻿using System;
using System.Collections.Generic;
using UnityEngine;
using XUnity.AutoTranslator.Plugin.Core.Endpoints;

namespace XUnity.AutoTranslator.Plugin.Core.UI
{
   internal class DropdownViewModel<TDropdownOptionViewModel, TSelection>
      where TDropdownOptionViewModel : DropdownOptionViewModel<TSelection>
      where TSelection : class
   {
      private Action<TSelection> _onSelected;

      public DropdownViewModel( IEnumerable<TDropdownOptionViewModel> options, Action<TSelection> onSelected )
      {
         _onSelected = onSelected;

         Options = new List<TDropdownOptionViewModel>();
         foreach( var item in options )
         {
            if( item.IsSelected() )
            {
               CurrentSelection = item;
            }
            Options.Add( item );
         }
      }

      public TDropdownOptionViewModel CurrentSelection { get; set; }

      public List<TDropdownOptionViewModel> Options { get; set; }

      public void Select( TDropdownOptionViewModel option )
      {
         if( option?.IsSelected() == true ) return;

         CurrentSelection = option;
         _onSelected?.Invoke( CurrentSelection?.Selection );
      }
   }

   internal class DropdownOptionViewModel<TSelection>
   {
      public DropdownOptionViewModel( string text, Func<bool> isSelected, Func<bool> isEnabled, TSelection selection )
      {
         Text = new GUIContent( text );
         IsSelected = isSelected;
         IsEnabled = isEnabled;
         Selection = selection;
      }

      public virtual GUIContent Text { get; set; }

      public Func<bool> IsEnabled { get; set; }

      public Func<bool> IsSelected { get; set; }

      public TSelection Selection { get; set; }
   }

   internal class TranslatorDropdownOptionViewModel : DropdownOptionViewModel<TranslationEndpointManager>
   {
      private GUIContent _selected;
      private GUIContent _normal;
      private GUIContent _disabled;

      public TranslatorDropdownOptionViewModel( Func<bool> isSelected, TranslationEndpointManager selection ) : base( selection.Endpoint.FriendlyName, isSelected, () => selection.Error == null, selection )
      {
         _selected = new GUIContent( selection.Endpoint.FriendlyName, $"<b>CURRENT TRANSLATOR</b>\n{selection.Endpoint.FriendlyName} is the currently selected translator that will be used to perform translations." );
         _disabled = new GUIContent( selection.Endpoint.FriendlyName, $"<b>CANNOT SELECT TRANSLATOR</b>\n{selection.Endpoint.FriendlyName} cannot be selected because the initialization failed. {selection.Error?.Message}" );
         _normal = new GUIContent( selection.Endpoint.FriendlyName, $"<b>SELECT TRANSLATOR</b>\n{selection.Endpoint.FriendlyName} will be selected as translator." );
      }

      public override GUIContent Text
      {
         get
         {
            if( Selection.Error != null )
            {
               return _disabled;
            }
            else if( IsSelected() )
            {
               return _selected;
            }
            else
            {
               return _normal;
            }
         }
      }
   }
}
