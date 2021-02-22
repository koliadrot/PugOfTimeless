using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    void Move();
    void Acceleration();
}
public interface ICommandable
{
    void Replay();
}
public interface IObserverable
{
    void Notify();
}

