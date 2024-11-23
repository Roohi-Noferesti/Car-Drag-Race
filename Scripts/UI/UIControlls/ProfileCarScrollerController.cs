using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using I2.Loc;
using UnityEngine;

public class ProfileCarScrollerController : CarouselScrollerController
{
    private ICarSimpleSpec[] m_cars;
    [SerializeField]
    private float m_cellViewSize = 190;


    public string SelectedID
    {
        get
        {
            if (m_cars != null && SelectedIndex > m_startDummyObjectCount - 1 && SelectedIndex < m_cars.Length + m_startDummyObjectCount)
                return m_cars[SelectedIndex - m_startDummyObjectCount].ID;
            return null;
        }
        set
        {
            if (m_cars != null)
            {
                Array.IndexOf(m_cars, value);
            }
        }
    }

    public void SetCars(IEnumerable<ICarSimpleSpec> cars)
    {
        m_cars = cars.ToArray();
        Reload();
    }
    public override int GetNumberOfCells(EnhancedScroller scroller)
    {
        if (m_cars != null)
            return m_cars.Length + DummyObjectCount;
        return 0;
    }

    public override float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return m_cellViewSize;
    }

    public override void UpdateCellData(EnhancedScrollerCellView cellView, int dataIndex, int cellIndex)
    {
        var carCellView = (cellView as UICarItemCellView);
        var car = m_cars[dataIndex];

        carCellView.SetActive(true);
        carCellView.Name = LocalizationManager.GetTranslation(CarDatabase.Instance.GetCarShortName(car.ID));
        carCellView.Rate = car.PPIndex;
        carCellView.CarID = car.ID;
        carCellView.ReloadImage(false);
        carCellView.IsSelected = SelectedIndex == dataIndex;
    }

    public void SetSelectedToFirstCar()
    {
        SelectedIndex = m_startDummyObjectCount;
    }
}
