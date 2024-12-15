using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadCardPresenter : CardPresenterBase
{
    public DeadCardPresenter(Card model, CardView view) : base(model, view)
    {
    }

    public override void Clear()
    {
        base.Clear();

        _view.gameObject.DestroyOrReturnToPool();
    }
}
