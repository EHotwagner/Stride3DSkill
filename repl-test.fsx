#!/usr/bin/env dotnet fsi
/// F# REPL test script for Stride3D usability.
/// Run with: dotnet fsi repl-test.fsx
///
/// This script tests that Stride3D types can be used interactively from F#.
/// It does NOT open a game window – it validates API surface, type construction,
/// and functional patterns work from the REPL.

#r "nuget: Stride.CommunityToolkit, 1.0.0-preview.62"
#r "nuget: Stride.CommunityToolkit.Bepu, 1.0.0-preview.62"
#r "nuget: Stride.CommunityToolkit.Skyboxes, 1.0.0-preview.62"

open Stride.Core.Mathematics
open Stride.Engine
open Stride.UI
open Stride.UI.Controls
open Stride.UI.Panels

printfn "=== Stride3D F# REPL Usability Tests ==="
printfn ""

// ---------------------------------------------------------------------------
// Test 1: Vector3 math
// ---------------------------------------------------------------------------
printfn "[Test 1] Vector3 construction and math"
let mutable v1 = Vector3(1f, 2f, 3f)
let mutable v2 = Vector3(4f, 5f, 6f)
let mutable result = Unchecked.defaultof<Vector3>
Vector3.Add(&v1, &v2, &result)
assert (result.X = 5f && result.Y = 7f && result.Z = 9f)
printfn "  PASS: Vector3.Add works"

let mutable scaled = Unchecked.defaultof<Vector3>
Vector3.Multiply(&v1, 3f, &scaled)
assert (scaled.X = 3f && scaled.Y = 6f && scaled.Z = 9f)
printfn "  PASS: Vector3.Multiply works"

// ---------------------------------------------------------------------------
// Test 2: Color construction
// ---------------------------------------------------------------------------
printfn "[Test 2] Color construction"
let c1 = Color(byte 255, byte 0, byte 0, byte 255)
assert (c1.R = byte 255 && c1.G = byte 0)
let _ = Color.Gold
let _ = Color.Orange
printfn "  PASS: Colors constructible from F#"

// ---------------------------------------------------------------------------
// Test 3: Entity creation and transform
// ---------------------------------------------------------------------------
printfn "[Test 3] Entity and Transform"
let entity = new Entity("TestEntity")
entity.Transform.Position <- Vector3(10f, 20f, 30f)
assert (entity.Transform.Position.X = 10f)
assert (entity.Name = "TestEntity")
printfn "  PASS: Entity construction and transform work"

// ---------------------------------------------------------------------------
// Test 4: Scene management
// ---------------------------------------------------------------------------
printfn "[Test 4] Scene management"
let scene = new Scene()
let e1 = new Entity("E1")
let e2 = new Entity("E2")
e1.Scene <- scene
e2.Scene <- scene
assert (scene.Entities.Count = 2)
printfn "  PASS: Scene entity management works"

// ---------------------------------------------------------------------------
// Test 5: Parent-child hierarchy
// ---------------------------------------------------------------------------
printfn "[Test 5] Entity hierarchy"
let parent = new Entity("Parent")
let child = new Entity("Child")
parent.AddChild(child)
assert (parent.Transform.Children.Count = 1)
printfn "  PASS: Entity hierarchy works"

// ---------------------------------------------------------------------------
// Test 6: UI types
// ---------------------------------------------------------------------------
printfn "[Test 6] UI construction"
let canvas = new Canvas(Width = 300f, Height = 100f)
let tb = TextBlock(Text = "Hello Stride from F#!", TextColor = Color.White, TextSize = 20f)
canvas.Children.Add(tb)
assert (canvas.Children.Count = 1)
assert (tb.Text = "Hello Stride from F#!")
printfn "  PASS: UI types work from F#"

// ---------------------------------------------------------------------------
// Test 7: Thickness construction
// ---------------------------------------------------------------------------
printfn "[Test 7] Thickness construction"
let t = Thickness(5f, 10f, 15f, 20f)
assert (t.Left = 5f && t.Top = 10f && t.Right = 15f && t.Bottom = 20f)
let tu = Thickness.UniformRectangle(8f)
assert (tu.Left = 8f && tu.Top = 8f)
printfn "  PASS: Thickness works"

// ---------------------------------------------------------------------------
// Test 8: Quaternion and rotation
// ---------------------------------------------------------------------------
printfn "[Test 8] Quaternion rotation"
let q = Quaternion.RotationX(MathUtil.DegreesToRadians(90f))
assert (q <> Quaternion.Identity)
printfn "  PASS: Quaternion rotation works"

// ---------------------------------------------------------------------------
// Test 9: Component system
// ---------------------------------------------------------------------------
printfn "[Test 9] Component system"
let compEntity = new Entity()
let uiComp = UIComponent()
compEntity.Add(uiComp)
let retrieved = compEntity.Get<UIComponent>()
assert (not (isNull retrieved))
printfn "  PASS: Component add/get works"

// ---------------------------------------------------------------------------
// Test 10: Functional patterns with Stride types
// ---------------------------------------------------------------------------
printfn "[Test 10] Functional patterns"

// Map over entities
let entities =
    [ for i in 0..4 ->
        let e = new Entity(sprintf "Obj%d" i)
        e.Transform.Position <- Vector3(float32 i * 2f, 0f, 0f)
        e ]

let positions = entities |> List.map (fun e -> e.Transform.Position.X)
assert (positions = [ 0f; 2f; 4f; 6f; 8f ])

// Filter entities
let farEntities = entities |> List.filter (fun e -> e.Transform.Position.X > 3f)
assert (farEntities.Length = 3)

// Fold to sum positions
let totalX =
    entities
    |> List.fold (fun acc e -> acc + e.Transform.Position.X) 0f
assert (totalX = 20f)
printfn "  PASS: Functional patterns work with Stride types"

// ---------------------------------------------------------------------------
// Summary
// ---------------------------------------------------------------------------
printfn ""
printfn "=== All 10 REPL tests passed! ==="
printfn "Stride3D is usable from F# interactive."
printfn ""
printfn "Note: To run the actual game with rendering, use:"
printfn "  cd Stride3dFSharp && dotnet run"
printfn "This requires a GPU and display server (X11/Wayland)."
