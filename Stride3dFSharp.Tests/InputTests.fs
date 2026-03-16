/// Tests for the functional input handling module.
module Stride3dFSharp.Tests.InputTests

open Xunit
open Stride.Core.Mathematics
open Stride3dFSharp.InputHandler

// ---------------------------------------------------------------------------
// Action to direction mapping
// ---------------------------------------------------------------------------

[<Fact>]
let ``MoveLeft maps to negative X`` () =
    let d = actionToDirection MoveLeft
    Assert.True(d.X < 0f)
    Assert.Equal(0f, d.Y)
    Assert.Equal(0f, d.Z)

[<Fact>]
let ``MoveRight maps to positive X`` () =
    let d = actionToDirection MoveRight
    Assert.True(d.X > 0f)

[<Fact>]
let ``MoveForward maps to negative Z`` () =
    let d = actionToDirection MoveForward
    Assert.True(d.Z < 0f)

[<Fact>]
let ``MoveBack maps to positive Z`` () =
    let d = actionToDirection MoveBack
    Assert.True(d.Z > 0f)

[<Fact>]
let ``Jump maps to positive Y`` () =
    let d = actionToDirection Jump
    Assert.True(d.Y > 0f)

[<Fact>]
let ``SpawnCube maps to zero vector`` () =
    let d = actionToDirection SpawnCube
    Assert.Equal(Vector3.Zero.X, d.X)

[<Fact>]
let ``NoAction maps to zero vector`` () =
    let d = actionToDirection NoAction
    Assert.Equal(Vector3.Zero.X, d.X)
    Assert.Equal(Vector3.Zero.Y, d.Y)
    Assert.Equal(Vector3.Zero.Z, d.Z)

// ---------------------------------------------------------------------------
// Movement computation
// ---------------------------------------------------------------------------

[<Fact>]
let ``computeMovement with no actions returns zero`` () =
    let result = computeMovement []
    Assert.Equal(0f, result.X)
    Assert.Equal(0f, result.Y)
    Assert.Equal(0f, result.Z)

[<Fact>]
let ``computeMovement with single direction is normalised`` () =
    let result = computeMovement [ MoveLeft ]
    let len = result.Length()
    Assert.True(abs (len - 1f) < 0.001f)

[<Fact>]
let ``computeMovement with opposing directions cancels out`` () =
    let result = computeMovement [ MoveLeft; MoveRight ]
    Assert.Equal(0f, result.X)

[<Fact>]
let ``computeMovement with diagonal is normalised`` () =
    let result = computeMovement [ MoveLeft; MoveForward ]
    let len = result.Length()
    Assert.True(abs (len - 1f) < 0.001f, sprintf "Expected ~1.0 but got %f" len)

[<Fact>]
let ``computeMovement with all four directions returns zero`` () =
    let result = computeMovement [ MoveLeft; MoveRight; MoveForward; MoveBack ]
    Assert.Equal(0f, result.X)
    Assert.Equal(0f, result.Z)

// ---------------------------------------------------------------------------
// hasAction
// ---------------------------------------------------------------------------

[<Fact>]
let ``hasAction finds present action`` () =
    Assert.True(hasAction Jump [ MoveLeft; Jump; MoveRight ])

[<Fact>]
let ``hasAction returns false for absent action`` () =
    Assert.False(hasAction SpawnCube [ MoveLeft; Jump ])

[<Fact>]
let ``hasAction on empty list returns false`` () =
    Assert.False(hasAction MoveLeft [])

// ---------------------------------------------------------------------------
// MovementAction discriminated union completeness
// ---------------------------------------------------------------------------

[<Fact>]
let ``All MovementAction cases can be converted to direction`` () =
    let allActions = [ MoveLeft; MoveRight; MoveForward; MoveBack; Jump; SpawnCube; NoAction ]
    let directions = allActions |> List.map actionToDirection
    Assert.Equal(7, directions.Length)
