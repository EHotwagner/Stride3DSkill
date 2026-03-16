/// Functional input handling – maps keyboard/mouse state to game actions.
module Stride3dFSharp.InputHandler

open Stride.Core.Mathematics
open Stride.Engine
open Stride.Input
open Stride.Games

// ---------------------------------------------------------------------------
// Action types – what the player can do
// ---------------------------------------------------------------------------

type MovementAction =
    | MoveLeft
    | MoveRight
    | MoveForward
    | MoveBack
    | Jump
    | SpawnCube
    | NoAction

// ---------------------------------------------------------------------------
// Input mapping – pure function from input state to actions
// ---------------------------------------------------------------------------

/// Poll the input manager and return a list of active actions this frame.
let pollActions (input: InputManager) =
    [ if input.IsKeyDown(Keys.A) || input.IsKeyDown(Keys.Left) then MoveLeft
      if input.IsKeyDown(Keys.D) || input.IsKeyDown(Keys.Right) then MoveRight
      if input.IsKeyDown(Keys.W) || input.IsKeyDown(Keys.Up) then MoveForward
      if input.IsKeyDown(Keys.S) || input.IsKeyDown(Keys.Down) then MoveBack
      if input.IsKeyPressed(Keys.Space) then Jump
      if input.IsKeyDown(Keys.Q) then SpawnCube ]

/// Convert a movement action to a direction vector.
let actionToDirection =
    function
    | MoveLeft -> Vector3(-1f, 0f, 0f)
    | MoveRight -> Vector3(1f, 0f, 0f)
    | MoveForward -> Vector3(0f, 0f, -1f)
    | MoveBack -> Vector3(0f, 0f, 1f)
    | Jump -> Vector3(0f, 1f, 0f)
    | SpawnCube -> Vector3.Zero
    | NoAction -> Vector3.Zero

/// Add two Vector3 values (workaround for Stride's inref<Vector3> operator).
let inline addVec3 (a: Vector3) (b: Vector3) =
    let mutable result = Unchecked.defaultof<Vector3>
    Vector3.Add(&a, &b, &result)
    result

/// Multiply a Vector3 by a scalar.
let inline scaleVec3 (s: float32) (v: Vector3) =
    let mutable result = Unchecked.defaultof<Vector3>
    Vector3.Multiply(&v, s, &result)
    result

/// Sum all movement actions into a single normalised direction.
let computeMovement (actions: MovementAction list) =
    let sum =
        actions
        |> List.map actionToDirection
        |> List.fold addVec3 Vector3.Zero

    if sum.LengthSquared() > 0f then
        let mutable n = sum
        n.Normalize()
        n
    else
        Vector3.Zero

// ---------------------------------------------------------------------------
// Applying movement to entities
// ---------------------------------------------------------------------------

/// Move an entity by direct transform manipulation (non-physical).
let applyTransformMovement (speed: float32) (deltaTime: float32) (direction: Vector3) (entity: Entity) =
    let offset = scaleVec3 (speed * deltaTime) direction
    entity.Transform.Position <- addVec3 entity.Transform.Position offset

/// Check if a specific action is present in the action list.
let hasAction action actions =
    List.contains action actions
