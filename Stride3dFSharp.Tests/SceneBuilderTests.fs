/// Tests for the declarative scene builder.
module Stride3dFSharp.Tests.SceneBuilderTests

open Xunit
open Stride.Core.Mathematics
open Stride.CommunityToolkit.Rendering.ProceduralModels
open Stride3dFSharp.SceneBuilder

// ---------------------------------------------------------------------------
// Default scene setup
// ---------------------------------------------------------------------------

[<Fact>]
let ``defaultScene has all features enabled`` () =
    Assert.True(defaultScene.UseCompositor)
    Assert.True(defaultScene.UseCamera)
    Assert.True(defaultScene.UseLight)
    Assert.True(defaultScene.UseGround)
    Assert.True(defaultScene.UseSkybox)
    Assert.False(defaultScene.UseProfiler)
    Assert.Empty(defaultScene.Primitives)

// ---------------------------------------------------------------------------
// Builder functions
// ---------------------------------------------------------------------------

[<Fact>]
let ``withProfiler enables profiler`` () =
    let setup = defaultScene |> withProfiler
    Assert.True(setup.UseProfiler)

[<Fact>]
let ``withoutSkybox disables skybox`` () =
    let setup = defaultScene |> withoutSkybox
    Assert.False(setup.UseSkybox)

[<Fact>]
let ``withoutGround disables ground`` () =
    let setup = defaultScene |> withoutGround
    Assert.False(setup.UseGround)

// ---------------------------------------------------------------------------
// Adding primitives
// ---------------------------------------------------------------------------

[<Fact>]
let ``addPrimitive appends to primitives list`` () =
    let setup =
        defaultScene
        |> addPrimitive PrimitiveModelType.Capsule (Vector3(0f, 5f, 0f))
    Assert.Equal(1, setup.Primitives.Length)
    Assert.Equal(PrimitiveModelType.Capsule, setup.Primitives.[0].PrimitiveType)
    Assert.True(setup.Primitives.[0].HasCollider)
    Assert.True(setup.Primitives.[0].Color.IsNone)

[<Fact>]
let ``addColouredPrimitive sets color and collider`` () =
    let setup =
        defaultScene
        |> addColouredPrimitive PrimitiveModelType.Cube (Vector3.Zero) Color.Red
    Assert.Equal(1, setup.Primitives.Length)
    Assert.True(setup.Primitives.[0].Color.IsSome)
    Assert.True(setup.Primitives.[0].HasCollider)

[<Fact>]
let ``addDecorativePrimitive has no collider`` () =
    let setup =
        defaultScene
        |> addDecorativePrimitive PrimitiveModelType.Sphere (Vector3.Zero) Color.Blue
    Assert.Equal(1, setup.Primitives.Length)
    Assert.False(setup.Primitives.[0].HasCollider)

// ---------------------------------------------------------------------------
// Composing multiple primitives
// ---------------------------------------------------------------------------

[<Fact>]
let ``multiple primitives accumulate in order`` () =
    let setup =
        defaultScene
        |> addPrimitive PrimitiveModelType.Capsule (Vector3(0f, 5f, 0f))
        |> addColouredPrimitive PrimitiveModelType.Cube (Vector3(1f, 0f, 0f)) Color.Gold
        |> addDecorativePrimitive PrimitiveModelType.Sphere (Vector3(2f, 0f, 0f)) Color.Green

    Assert.Equal(3, setup.Primitives.Length)
    Assert.Equal(PrimitiveModelType.Capsule, setup.Primitives.[0].PrimitiveType)
    Assert.Equal(PrimitiveModelType.Cube, setup.Primitives.[1].PrimitiveType)
    Assert.Equal(PrimitiveModelType.Sphere, setup.Primitives.[2].PrimitiveType)

[<Fact>]
let ``primitives preserve position`` () =
    let pos = Vector3(3f, 7f, -2f)
    let setup =
        defaultScene
        |> addPrimitive PrimitiveModelType.Capsule pos
    Assert.Equal(pos, setup.Primitives.[0].Position)

// ---------------------------------------------------------------------------
// Builder pipeline preserves other settings
// ---------------------------------------------------------------------------

[<Fact>]
let ``adding primitives does not change compositor/camera/light settings`` () =
    let setup =
        defaultScene
        |> withoutSkybox
        |> withProfiler
        |> addPrimitive PrimitiveModelType.Capsule Vector3.Zero

    Assert.True(setup.UseCompositor)
    Assert.True(setup.UseCamera)
    Assert.True(setup.UseLight)
    Assert.True(setup.UseGround)
    Assert.False(setup.UseSkybox)
    Assert.True(setup.UseProfiler)
    Assert.Equal(1, setup.Primitives.Length)

// ---------------------------------------------------------------------------
// PrimitiveSpec record equality
// ---------------------------------------------------------------------------

[<Fact>]
let ``PrimitiveSpec records with same values are equal`` () =
    let a =
        { PrimitiveType = PrimitiveModelType.Cube
          Position = Vector3(1f, 2f, 3f)
          Color = Some Color.Red
          HasCollider = true }
    let b =
        { PrimitiveType = PrimitiveModelType.Cube
          Position = Vector3(1f, 2f, 3f)
          Color = Some Color.Red
          HasCollider = true }
    Assert.Equal(a, b)
