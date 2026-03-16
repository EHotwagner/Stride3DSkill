/// Bepu physics helpers for F#.
module Stride3dFSharp.PhysicsHelper

open Stride.Core.Mathematics
open Stride.Engine
open Stride.BepuPhysics
open Stride.CommunityToolkit.Bepu
open Stride.CommunityToolkit.Helpers

// ---------------------------------------------------------------------------
// Impulse helpers
// ---------------------------------------------------------------------------

/// Apply an impulse to an entity's BodyComponent if it has one.
let applyImpulse (impulse: Vector3) (entity: Entity) =
    let body = entity.Get<BodyComponent>()
    if not (isNull body) then
        body.Awake <- true
        body.ApplyImpulse(impulse, Vector3.Zero)

/// Apply an upward impulse (e.g. jump).
let applyJump (force: float32) (entity: Entity) =
    applyImpulse (Vector3(0f, force, 0f)) entity

/// Apply a random impulse (useful for scattering objects).
let applyRandomImpulse (minMax: float32) (entity: Entity) =
    let dir =
        VectorHelper.RandomVector3(
            [| -minMax; minMax |],
            [| 0f; minMax |],
            [| -minMax; minMax |]
        )
    applyImpulse dir entity

// ---------------------------------------------------------------------------
// Raycasting helpers
// ---------------------------------------------------------------------------

/// Raycast from a camera at a screen position. Returns the hit entity if any.
let raycastFromCamera (camera: CameraComponent) (screenPos: Vector2) (maxDistance: float32) =
    let mutable hitInfo = Unchecked.defaultof<HitInfo>
    if camera.Raycast(screenPos, maxDistance, &hitInfo) then
        Some hitInfo.Collidable.Entity
    else
        None

// ---------------------------------------------------------------------------
// Query helpers
// ---------------------------------------------------------------------------

/// Get the BodyComponent from an entity, if present.
let tryGetBody (entity: Entity) =
    entity.Get<BodyComponent>() |> Option.ofObj

/// Check whether an entity has physics.
let hasPhysics (entity: Entity) =
    entity.Get<BodyComponent>() |> isNull |> not
