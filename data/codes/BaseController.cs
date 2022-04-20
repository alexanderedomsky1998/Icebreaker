using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;
using Console = Unigine.Console;
using Random = Unigine.Random;

[Component(PropertyGuid = "ab6d46a3610b7ba6d07e5ecd6ea471c87ca66a87")]
public class BaseController : Component
{
    /*
     * Группа переменных отвечающих за действия,
     * которые доступны для настройки в редакторе
     */
    [ShowInEditor] [Parameter(Group = "Input", Tooltip = "Move forward key")]
    private Input.KEY forwardKey = Input.KEY.W;

    [ShowInEditor] [Parameter(Group = "Input", Tooltip = "Move backward key")]
    private Input.KEY backwardKey = Input.KEY.S;

    [ShowInEditor] [Parameter(Group = "Input", Tooltip = "Move right key")]
    private Input.KEY rightKey = Input.KEY.D;

    [ShowInEditor] [Parameter(Group = "Input", Tooltip = "Move left key")]
    private Input.KEY leftKey = Input.KEY.A;

    /*
     * Внутренние переменные отвечающие за текущее направление корабля
     * исходя из ввода
     */
    private float forwardInputSpeed;
    private float turnSpeed;

    /*
     * Константы влияющие на внутреннюю логику работы мотора
     */
    private const float MaxForwardSpeed = 10.0f;
    private const float ForwardAcceleration = MaxForwardSpeed / 2.5f;

    // Переменная которая отвечает за текущую скорость мотора
    private float motorForwardSpeed = 0.0f;

    /*
     * Группа вспомогательных переменных для внутренней логики
     */
    private BodyDummy body;

    private readonly List<int> listIces = new();

    private readonly int countTouches = (int)MathLib.RandInt(100, 200);

    /*
     * Метод инициализации компонента,
     * вызывается при создании объекта-компонента
     *
     * В рамках данного метода идет поиск объект
     * физики корабля для будущего взаимодействия
     */
    private void Init()
    {
        // Debug
        if (false)
        {
            Console.Run("console_onscreen 1");
        }

        // write here code to be called on component initialization
        var indexBody = node.FindChild("Body");
        var child = node.GetChild(indexBody);
        body = child.ObjectBody as BodyDummy;
    }

    /*
     * Метод обновления компонента, вызывается каждый кадр
     * для каждого объекта-компонента
     *
     * В данном методе вызывается обновление ввода,
     * затем обновление текущего состояния движения объекта
     * и после этого само перемещение объекта в рамках данного кадра
     */
    private void Update()
    {
        UpdateMovement();
        UpdateMotor();
        node.Translate(new vec3(0f, motorForwardSpeed * Game.IFps, 0.0f));
        node.Rotate(0.0f, 0.0f, turnSpeed * Game.IFps * 5);
    }

    /*
     * Метод отвечающий за получение ввода от пользователя
     * и обновления направлений движения и проверку взаимодействия с льдинами
     */
    private void UpdateMovement()
    {
        if (Input.IsKeyPressed(forwardKey))
            AddMovementDirection(1.0f);

        if (Input.IsKeyPressed(backwardKey))
            AddMovementDirection(-1.0f);

        if (!Input.IsKeyPressed(forwardKey) && !Input.IsKeyPressed(backwardKey))
            forwardInputSpeed = 0.0f;

        if (Input.IsKeyPressed(rightKey))
            AddTurnSpeed(-1f);

        if (Input.IsKeyPressed(leftKey))
            AddTurnSpeed(1f);

        if (!Input.IsKeyPressed(rightKey) && !Input.IsKeyPressed(leftKey))
            turnSpeed = 0.0f;

        for (var i = 0; i < body.NumContacts; i++)
        {
            var result = listIces.IndexOf(body.GetContactBody0(i).ID);
            if (result == -1)
            {
                listIces.Add(body.GetContactBody0(i).ID);
                TakeDamage();
            }
        }
    }

    /*
     * Метод обновления направления желаемого
     * движения пользователя вперед либо назад
     */
    private void AddMovementDirection(float diff)
    {
        forwardInputSpeed = MathLib.Clamp(forwardInputSpeed + diff, -1f, 1f);
    }

    /*
     * Метод обновления желаемого движения пользователем влево либо вправо
     */
    private void AddTurnSpeed(float diff)
    {
        turnSpeed = Math.Clamp(turnSpeed + diff, -1f, 1f);
    }

    /*
     * Метод обновления мощности мотора,
     * внутри данного метода корабль умеет набирать скорость
     * исходя из ввода и константы ускорения.
     * Также в этом коде используется ограничение на максимальную скорость,
     * но это пришло из прошлых итераций.
     */
    private void UpdateMotor()
    {
        var newMotorSpeed = motorForwardSpeed + ForwardAcceleration * Game.IFps * forwardInputSpeed;
        var newMaxSpeed = MaxForwardSpeed * GetCurrentPowerPercent();
        motorForwardSpeed = Math.Clamp(newMotorSpeed, -newMaxSpeed, newMaxSpeed);
        Log.Message(
            $"InputSpeed: {forwardInputSpeed}, " +
            $"MotorForwardSpeed: {motorForwardSpeed}, " +
            $"newMaxSpeed: {newMaxSpeed}, " +
            $"ListIces.Count: {listIces.Count}\n");
    }

    /*
     * Метод получения текущей максимальной мощности мотора.
     * Ранее она использовалась для того чтобы ограничить
     * максимальную скорость корабля исходя из его повреждений.
     * На текущий момент не имеет значения.
     */
    private float GetCurrentPowerPercent()
    {
        return 1.0f;
        // Bug - there are sometimes 2 objects anyway
        var numContacts = body.NumContacts;

        return listIces.Count switch
        {
            <= 20 => 1.0f,
            <= 50 => 0.8f,
            <= 70 => 0.5f,
            <= 75 => 0.25f,
            <= 80 => 0.125f,
            <= 90 => 0.0f,
            _ => 0.0f
        };
    }

    /*
     * Метод получения урона, данный метод замедляет корабль
     * для будущей полной остановки
     */
    private void TakeDamage()
    {
        motorForwardSpeed -= MathLib.Clamp(MaxForwardSpeed / countTouches, 0.0f, MaxForwardSpeed);
        if (motorForwardSpeed <= 1.0f)
        {
            motorForwardSpeed = 0.0f;
        }
    }
}