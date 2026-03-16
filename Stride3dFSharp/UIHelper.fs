/// UI construction helpers for Stride3D from F#.
module Stride3dFSharp.UIHelper

open Stride.Core.Mathematics
open Stride.Engine
open Stride.Graphics
open Stride.Rendering
open Stride.UI
open Stride.UI.Controls
open Stride.UI.Panels

// ---------------------------------------------------------------------------
// Text overlay
// ---------------------------------------------------------------------------

/// Create a text block entity anchored at the given alignment.
let createTextOverlay
    (font: SpriteFont)
    (text: string)
    (hAlign: HorizontalAlignment)
    (vAlign: VerticalAlignment)
    =
    let canvas =
        new Canvas(
            Width = 350f,
            Height = 60f,
            BackgroundColor = Color(byte 30, byte 30, byte 30, byte 180),
            HorizontalAlignment = hAlign,
            VerticalAlignment = vAlign
        )

    let tb =
        TextBlock(
            Text = text,
            TextColor = Color.White,
            Font = font,
            TextSize = 20f,
            Margin = Thickness(5f, 5f, 5f, 0f)
        )

    canvas.Children.Add(tb)

    let entity = new Entity("UIOverlay")
    entity.Add(
        UIComponent(
            Page = new UIPage(RootElement = canvas),
            RenderGroup = RenderGroup.Group31
        )
    )
    entity, tb

/// Update the text of a TextBlock.
let updateText (tb: TextBlock) (text: string) =
    tb.Text <- text

// ---------------------------------------------------------------------------
// Simple HUD with multiple lines
// ---------------------------------------------------------------------------

/// Create a multi-line HUD panel.
let createHud (font: SpriteFont) (lines: string list) =
    let panel =
        new StackPanel(
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = Thickness(10f, 10f, 10f, 10f)
        )

    let blocks =
        lines
        |> List.map (fun line ->
            let tb =
                TextBlock(
                    Text = line,
                    TextColor = Color.White,
                    Font = font,
                    TextSize = 16f,
                    Margin = Thickness(2f, 2f, 2f, 2f)
                )
            panel.Children.Add(tb)
            tb)

    let entity = new Entity("HUD")
    entity.Add(
        UIComponent(
            Page = new UIPage(RootElement = panel),
            RenderGroup = RenderGroup.Group31
        )
    )
    entity, blocks
