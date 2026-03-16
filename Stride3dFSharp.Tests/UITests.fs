/// Tests for UI construction helpers.
module Stride3dFSharp.Tests.UITests

open Xunit
open Stride.Core.Mathematics
open Stride.UI
open Stride.UI.Controls
open Stride.UI.Panels
open Stride.Engine

// ---------------------------------------------------------------------------
// Stride UI types can be instantiated from F#
// ---------------------------------------------------------------------------

[<Fact>]
let ``TextBlock can be created`` () =
    let tb = TextBlock(Text = "Hello", TextColor = Color.White, TextSize = 20f)
    Assert.Equal("Hello", tb.Text)
    Assert.Equal(20f, tb.TextSize)

[<Fact>]
let ``Canvas can be created with dimensions`` () =
    let canvas = new Canvas(Width = 300f, Height = 100f)
    Assert.Equal(300f, canvas.Width)
    Assert.Equal(100f, canvas.Height)

[<Fact>]
let ``Canvas children can be added`` () =
    let canvas = new Canvas()
    let tb = TextBlock(Text = "Test")
    canvas.Children.Add(tb)
    Assert.Equal(1, canvas.Children.Count)

[<Fact>]
let ``StackPanel can be created with orientation`` () =
    let panel = new StackPanel(Orientation = Orientation.Vertical)
    Assert.Equal(Orientation.Vertical, panel.Orientation)

[<Fact>]
let ``Thickness can be constructed with 4 params`` () =
    let t = Thickness(1f, 2f, 3f, 4f)
    Assert.Equal(1f, t.Left)
    Assert.Equal(2f, t.Top)
    Assert.Equal(3f, t.Right)
    Assert.Equal(4f, t.Bottom)

[<Fact>]
let ``Thickness UniformRectangle creates equal margins`` () =
    let t = Thickness.UniformRectangle(5f)
    Assert.Equal(5f, t.Left)
    Assert.Equal(5f, t.Top)
    Assert.Equal(5f, t.Right)
    Assert.Equal(5f, t.Bottom)

// ---------------------------------------------------------------------------
// UIPage and UIComponent
// ---------------------------------------------------------------------------

[<Fact>]
let ``UIPage can wrap a root element`` () =
    let canvas = new Canvas()
    let page = new UIPage(RootElement = canvas)
    Assert.Same(canvas, page.RootElement)

[<Fact>]
let ``UIComponent can be created with a page`` () =
    let canvas = new Canvas()
    let page = new UIPage(RootElement = canvas)
    let comp = Stride.Engine.UIComponent(Page = page)
    Assert.Same(page, comp.Page)

// ---------------------------------------------------------------------------
// Alignment enums
// ---------------------------------------------------------------------------

[<Fact>]
let ``HorizontalAlignment values are accessible`` () =
    let _ = HorizontalAlignment.Left
    let _ = HorizontalAlignment.Center
    let _ = HorizontalAlignment.Right
    let _ = HorizontalAlignment.Stretch
    Assert.True(true)

[<Fact>]
let ``VerticalAlignment values are accessible`` () =
    let _ = VerticalAlignment.Top
    let _ = VerticalAlignment.Center
    let _ = VerticalAlignment.Bottom
    let _ = VerticalAlignment.Stretch
    Assert.True(true)
