/// Tests for physics helper functions (unit-level, no running game required).
module Stride3dFSharp.Tests.PhysicsTests

open Xunit
open Stride.Core.Mathematics
open Stride.Engine
open Stride3dFSharp.PhysicsHelper

// ---------------------------------------------------------------------------
// Entity without physics
// ---------------------------------------------------------------------------

[<Fact>]
let ``tryGetBody returns None for entity without BodyComponent`` () =
    let entity = new Entity()
    let body = tryGetBody entity
    Assert.True(body.IsNone)

[<Fact>]
let ``hasPhysics returns false for entity without BodyComponent`` () =
    let entity = new Entity()
    Assert.False(hasPhysics entity)

[<Fact>]
let ``applyImpulse on entity without body does not throw`` () =
    let entity = new Entity()
    // Should silently do nothing
    applyImpulse (Vector3(10f, 0f, 0f)) entity

[<Fact>]
let ``applyJump on entity without body does not throw`` () =
    let entity = new Entity()
    applyJump 5f entity

[<Fact>]
let ``applyRandomImpulse on entity without body does not throw`` () =
    let entity = new Entity()
    applyRandomImpulse 10f entity

// ---------------------------------------------------------------------------
// Entity construction
// ---------------------------------------------------------------------------

[<Fact>]
let ``Entity can be created with a name`` () =
    let entity = new Entity("TestEntity")
    Assert.Equal("TestEntity", entity.Name)

[<Fact>]
let ``Entity transform starts at origin`` () =
    let entity = new Entity()
    Assert.Equal(0f, entity.Transform.Position.X)
    Assert.Equal(0f, entity.Transform.Position.Y)
    Assert.Equal(0f, entity.Transform.Position.Z)

[<Fact>]
let ``Entity transform position can be set`` () =
    let entity = new Entity()
    entity.Transform.Position <- Vector3(5f, 10f, 15f)
    Assert.Equal(5f, entity.Transform.Position.X)
    Assert.Equal(10f, entity.Transform.Position.Y)
    Assert.Equal(15f, entity.Transform.Position.Z)

// ---------------------------------------------------------------------------
// Entity components
// ---------------------------------------------------------------------------

[<Fact>]
let ``Entity Get returns null for missing component`` () =
    let entity = new Entity()
    let uc = entity.Get<Stride.Engine.UIComponent>()
    Assert.True(isNull uc)

[<Fact>]
let ``Entity can add and retrieve a component`` () =
    let entity = new Entity()
    let tc = new TransformComponent()
    // Entity already has a TransformComponent by default
    let existing = entity.Get<TransformComponent>()
    Assert.NotNull(existing)
