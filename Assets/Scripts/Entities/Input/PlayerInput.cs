using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInput : MonoBehaviour
{

  [SerializeField]
  private int playerId;

  private Player player;

  private Vector2 runDirection;
  private Vector2 aimDirection;
  private bool pressedSkill1;
  private bool pressedSkill2;
  private bool pressedInteract;
  private bool pressedDash;

  void Awake()
  {
    player = ReInput.players.GetPlayer(playerId);
  }

  public void GatherInput()
  {
    pressedSkill1 = _GetPressedSkill1();
    pressedSkill2 = _GetPressedSkill2();
    runDirection = _GetRunDirection();
    aimDirection = _GetAimDirection();
    pressedInteract = _GetPressedInteract();
    pressedDash = _GetPressedDash();
  }

  void LateUpdate()
  {
    pressedSkill1 = false;
    runDirection = Vector2.zero;
    aimDirection = Vector2.zero;
    pressedInteract = false;
    pressedDash = false;
  }

  #region Getters
  public bool DidPressSkill1()
  {
    return pressedSkill1;
  }

  public bool DidPressSkill2()
  {
    return pressedSkill2;
  }

  public Vector2 GetRunDirection()
  {
    return runDirection;
  }

  public Vector2 GetAimDirection()
  {
    return aimDirection;
  }

  public bool DidPressInteract()
  {
    return pressedInteract;
  }

  public bool DidPressDash()
  {
    return pressedDash;
  }
  #endregion

  #region Get raw input

  private bool _GetPressedSkill1()
  {
    return player.GetButtonDown("Skill 1");
  }

  private bool _GetPressedSkill2()
  {
    return player.GetButtonDown("Skill 2");
  }

  private Vector2 _GetAimDirection()
  {
    float horizInput = player.GetAxis("Aim Horizontal"); ;
    float verticalInput = player.GetAxis("Aim Vertical");
    Vector2 direction = new Vector2(horizInput, verticalInput);
    return direction.normalized;
  }

  private Vector2 _GetRunDirection()
  {
    return new Vector2(player.GetAxis("Move Horizontal"), player.GetAxis("Move Vertical")).normalized;
  }

  private bool _GetPressedDash()
  {
    return player.GetButtonDown("Dash");
  }

  private bool _GetPressedInteract()
  {
    return player.GetButtonDown("Interact");
  }
  #endregion
}