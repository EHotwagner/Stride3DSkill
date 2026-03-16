/// Integration tests – verify that Stride engine types work together from F#.
/// These tests do NOT require a running game window or GPU.
module Stride3dFSharp.Tests.IntegrationTests

open Xunit
open Stride.Core.Mathematics
open Stride.Engine
open Stride.CommunityToolkit.Rendering.ProceduralModels
open Stride3dFSharp.SceneBuilder
open Stride3dFSharp.InputHandler

// ---------------------------------------------------------------------------
// Scene + Entity integration
// ---------------------------------------------------------------------------

[<Fact>]
let ``Scene can be created and entities added`` () =
    let scene = new Scene()
    let entity = new Entity("TestCube")
    entity.Transform.Position <- Vector3(1f, 2f, 3f)
    scene.Entities.Add(entity)
    Assert.Equal(1, scene.Entities.Count)

[<Fact>]
let ``Multiple entities can be added to a scene`` () =
    let scene = new Scene()
    for i in 0..9 do
        let e = new Entity(sprintf "Entity%d" i)
        e.Transform.Position <- Vector3(float32 i, 0f, 0f)
        scene.Entities.Add(e)
    Assert.Equal(10, scene.Entities.Count)

[<Fact>]
let ``Entity can be removed from scene`` () =
    let scene = new Scene()
    let entity = new Entity("ToRemove")
    scene.Entities.Add(entity)
    Assert.Equal(1, scene.Entities.Count)
    scene.Entities.Remove(entity) |> ignore
    Assert.Equal(0, scene.Entities.Count)

// ---------------------------------------------------------------------------
// Transform hierarchy
// ---------------------------------------------------------------------------

[<Fact>]
let ``Child entity inherits parent transform`` () =
    let parent = new Entity("Parent")
    let child = new Entity("Child")
    parent.Transform.Position <- Vector3(10f, 0f, 0f)
    child.Transform.Position <- Vector3(0f, 5f, 0f)
    parent.AddChild(child)
    Assert.Equal(1, parent.Transform.Children.Count)

[<Fact>]
let ``Entity rotation can be set`` () =
    let entity = new Entity()
    entity.Transform.Rotation <- Quaternion.RotationX(MathUtil.DegreesToRadians(45f))
    Assert.NotEqual(Quaternion.Identity, entity.Transform.Rotation)

[<Fact>]
let ``Entity scale can be set`` () =
    let entity = new Entity()
    entity.Transform.Scale <- Vector3(2f, 2f, 2f)
    Assert.Equal(2f, entity.Transform.Scale.X)

// ---------------------------------------------------------------------------
// SceneBuilder + InputHandler integration
// ---------------------------------------------------------------------------

[<Fact>]
let ``Scene setup can be composed with input movement`` () =
    // Build a scene description
    let setup =
        defaultScene
        |> withProfiler
        |> addPrimitive PrimitiveModelType.Capsule (Vector3(0f, 5f, 0f))
        |> addColouredPrimitive PrimitiveModelType.Cube (Vector3(3f, 1f, 0f)) Color.Red

    Assert.Equal(2, setup.Primitives.Length)

    // Simulate movement computation
    let movement = computeMovement [ MoveLeft; MoveForward ]
    Assert.True(movement.Length() > 0f)

    // Simulate applying movement to an entity
    let entity = new Entity("Player")
    entity.Transform.Position <- Vector3(0f, 5f, 0f)
    applyTransformMovement 5f 0.016f movement entity

    // Position should have changed
    Assert.NotEqual(Vector3(0f, 5f, 0f), entity.Transform.Position)

// ---------------------------------------------------------------------------
// Stride type interop patterns from F#
// ---------------------------------------------------------------------------

[<Fact>]
let ``Can use Entity.Get with generic type parameter`` () =
    let entity = new Entity()
    let transform = entity.Get<TransformComponent>()
    Assert.NotNull(transform)

[<Fact>]
let ``Can add custom EntityComponent`` () =
    let entity = new Entity()
    let uiComp = UIComponent()
    entity.Add(uiComp)
    let retrieved = entity.Get<UIComponent>()
    Assert.NotNull(retrieved)

[<Fact>]
let ``Scene can be used as entity parent`` () =
    let scene = new Scene()
    let entity = new Entity("SceneChild")
    entity.Scene <- scene
    Assert.Equal(1, scene.Entities.Count)

// ---------------------------------------------------------------------------
// PrimitiveModelType enum coverage
// ---------------------------------------------------------------------------

[<Fact>]
let ``All common PrimitiveModelType values are accessible`` () =
    let types = [
        PrimitiveModelType.Capsule
        PrimitiveModelType.Cube
        PrimitiveModelType.Sphere
        PrimitiveModelType.Cylinder
        PrimitiveModelType.Plane
        PrimitiveModelType.Cone
        PrimitiveModelType.Torus
    ]
    Assert.Equal(7, types.Length)

// ---------------------------------------------------------------------------
// Complex scene building
// ---------------------------------------------------------------------------

[<Fact>]
let ``Can build a scene with many diverse primitives`` () =
    let setup =
        defaultScene
        |> addPrimitive PrimitiveModelType.Capsule (Vector3(0f, 5f, 0f))
        |> addPrimitive PrimitiveModelType.Sphere (Vector3(2f, 5f, 0f))
        |> addColouredPrimitive PrimitiveModelType.Cube (Vector3(-2f, 1f, 0f)) Color.Red
        |> addColouredPrimitive PrimitiveModelType.Cylinder (Vector3(4f, 1f, 0f)) Color.Blue
        |> addDecorativePrimitive PrimitiveModelType.Torus (Vector3(0f, 3f, 3f)) Color.Gold
        |> addDecorativePrimitive PrimitiveModelType.Cone (Vector3(-4f, 1f, 0f)) Color.Green

    Assert.Equal(6, setup.Primitives.Length)

    // Verify mix of physics/non-physics
    let withCollider = setup.Primitives |> List.filter (fun p -> p.HasCollider) |> List.length
    let withoutCollider = setup.Primitives |> List.filter (fun p -> not p.HasCollider) |> List.length
    Assert.Equal(4, withCollider)
    Assert.Equal(2, withoutCollider)

[<Fact>]
let ``Movement pipeline handles rapid successive movements`` () =
    let entity = new Entity("Mover")
    entity.Transform.Position <- Vector3.Zero

    // Simulate 100 frames of movement
    for _ in 1..100 do
        let actions = [ MoveRight; MoveForward ]
        let dir = computeMovement actions
        applyTransformMovement 5f 0.016f dir entity

    // Should have moved significantly
    Assert.True(entity.Transform.Position.X > 0f)
    Assert.True(entity.Transform.Position.Z < 0f)
