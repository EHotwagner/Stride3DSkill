/// Tests for Vector3 math helpers and Stride.Core.Mathematics interop from F#.
module Stride3dFSharp.Tests.MathTests

open Xunit
open Stride.Core.Mathematics
open Stride3dFSharp.InputHandler

// ---------------------------------------------------------------------------
// Vector3 addition helper
// ---------------------------------------------------------------------------

[<Fact>]
let ``addVec3 adds two vectors correctly`` () =
    let a = Vector3(1f, 2f, 3f)
    let b = Vector3(4f, 5f, 6f)
    let result = addVec3 a b
    Assert.Equal(5f, result.X)
    Assert.Equal(7f, result.Y)
    Assert.Equal(9f, result.Z)

[<Fact>]
let ``addVec3 with zero returns same vector`` () =
    let v = Vector3(3f, 7f, -1f)
    let result = addVec3 v Vector3.Zero
    Assert.Equal(v.X, result.X)
    Assert.Equal(v.Y, result.Y)
    Assert.Equal(v.Z, result.Z)

[<Fact>]
let ``addVec3 is commutative`` () =
    let a = Vector3(1f, -2f, 3f)
    let b = Vector3(-4f, 5f, -6f)
    let r1 = addVec3 a b
    let r2 = addVec3 b a
    Assert.Equal(r1.X, r2.X)
    Assert.Equal(r1.Y, r2.Y)
    Assert.Equal(r1.Z, r2.Z)

// ---------------------------------------------------------------------------
// Vector3 scale helper
// ---------------------------------------------------------------------------

[<Fact>]
let ``scaleVec3 scales correctly`` () =
    let v = Vector3(1f, 2f, 3f)
    let result = scaleVec3 2f v
    Assert.Equal(2f, result.X)
    Assert.Equal(4f, result.Y)
    Assert.Equal(6f, result.Z)

[<Fact>]
let ``scaleVec3 by zero gives zero vector`` () =
    let v = Vector3(5f, 10f, 15f)
    let result = scaleVec3 0f v
    Assert.Equal(0f, result.X)
    Assert.Equal(0f, result.Y)
    Assert.Equal(0f, result.Z)

[<Fact>]
let ``scaleVec3 by negative inverts direction`` () =
    let v = Vector3(1f, 2f, 3f)
    let result = scaleVec3 -1f v
    Assert.Equal(-1f, result.X)
    Assert.Equal(-2f, result.Y)
    Assert.Equal(-3f, result.Z)

// ---------------------------------------------------------------------------
// Vector3 construction from F#
// ---------------------------------------------------------------------------

[<Fact>]
let ``Vector3 can be constructed with xyz`` () =
    let v = Vector3(1.5f, 2.5f, 3.5f)
    Assert.Equal(1.5f, v.X)
    Assert.Equal(2.5f, v.Y)
    Assert.Equal(3.5f, v.Z)

[<Fact>]
let ``Vector3.Zero is all zeros`` () =
    Assert.Equal(0f, Vector3.Zero.X)
    Assert.Equal(0f, Vector3.Zero.Y)
    Assert.Equal(0f, Vector3.Zero.Z)

[<Fact>]
let ``Vector3 unit vectors are correct`` () =
    Assert.Equal(1f, Vector3.UnitX.X)
    Assert.Equal(0f, Vector3.UnitX.Y)
    Assert.Equal(1f, Vector3.UnitY.Y)
    Assert.Equal(1f, Vector3.UnitZ.Z)

// ---------------------------------------------------------------------------
// Color construction
// ---------------------------------------------------------------------------

[<Fact>]
let ``Color can be constructed from bytes`` () =
    let c = Color(byte 255, byte 128, byte 0, byte 255)
    Assert.Equal(byte 255, c.R)
    Assert.Equal(byte 128, c.G)
    Assert.Equal(byte 0, c.B)

[<Fact>]
let ``Named colors are accessible`` () =
    let _ = Color.Red
    let _ = Color.Green
    let _ = Color.Blue
    let _ = Color.White
    let _ = Color.Gold
    let _ = Color.Orange
    Assert.True(true)

// ---------------------------------------------------------------------------
// Matrix construction
// ---------------------------------------------------------------------------

[<Fact>]
let ``Matrix Identity is accessible`` () =
    let m = Matrix.Identity
    Assert.Equal(1f, m.M11)
    Assert.Equal(0f, m.M12)
    Assert.Equal(1f, m.M22)

[<Fact>]
let ``Quaternion Identity is accessible`` () =
    let q = Quaternion.Identity
    Assert.Equal(0f, q.X)
    Assert.Equal(0f, q.Y)
    Assert.Equal(0f, q.Z)
    Assert.Equal(1f, q.W)
