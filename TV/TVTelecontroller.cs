using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BToolkit
{
    public class TVTelecontroller : MonoBehaviour
    {

        public enum Direction
        {
            Horizontal,
            Vertical
        }
        [Serializable]
        public class Group
        {
            public string name = "Group";
            public TVButton[] btns;
        }
        public bool canNextGroupWhenBtnEnd;
        public Direction groupsDirection = Direction.Vertical;
        public Group[] groups = new Group[1];
        public int currGroupIndex, currBtnIndex;

        void Start()
        {
            OnBtnIndexUpdate();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //箭头向上
                if (groupsDirection == Direction.Horizontal)
                {
                    BtnIndexAdd(false);
                }
                else
                {
                    GroupIndexAdd(false);
                }
                OnBtnIndexUpdate();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //箭头向下
                if (groupsDirection == Direction.Horizontal)
                {
                    BtnIndexAdd(true);
                }
                else
                {
                    GroupIndexAdd(true);
                }
                OnBtnIndexUpdate();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //箭头向左
                if (groupsDirection == Direction.Horizontal)
                {
                    GroupIndexAdd(false);
                }
                else
                {
                    BtnIndexAdd(false);
                }
                OnBtnIndexUpdate();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                //箭头向右
                if (groupsDirection == Direction.Horizontal)
                {
                    GroupIndexAdd(true);
                }
                else
                {
                    BtnIndexAdd(true);
                }
                OnBtnIndexUpdate();
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown((KeyCode)10))
            {
                groups[currGroupIndex].btns[currBtnIndex].OnPress();
            }
            else if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.JoystickButton0) || Input.GetKeyUp((KeyCode)10))
            {
                groups[currGroupIndex].btns[currBtnIndex].OnClick();
            }
        }

        /// <summary>
        /// 跳到下一组或上一组
        /// </summary>
        public void GroupIndexAdd(bool next)
        {
            if (next)
            {
                if (currGroupIndex < groups.Length - 1)
                {
                    currGroupIndex++;
                    currBtnIndex = 0;
                }
            }
            else
            {
                if (currGroupIndex > 0)
                {
                    currGroupIndex--;
                    currBtnIndex = groups[currGroupIndex].btns.Length - 1;
                }
            }
        }

        /// <summary>
        /// 在当前组里跳到下一个按钮或上一个按钮
        /// </summary>
        public void BtnIndexAdd(bool next)
        {
            if (next)
            {
                currBtnIndex++;
                int btnsCount = groups[currGroupIndex].btns.Length;
                if (currBtnIndex > btnsCount - 1)
                {
                    if (canNextGroupWhenBtnEnd && currGroupIndex < groups.Length - 1)
                    {
                        //到最后一个按钮时允许跳到下一组
                        GroupIndexAdd(true);
                        currBtnIndex = 0;
                    }
                    else
                    {
                        currBtnIndex = btnsCount - 1;
                    }
                }
            }
            else
            {
                currBtnIndex--;
                if (currBtnIndex < 0)
                {
                    if (canNextGroupWhenBtnEnd && currGroupIndex > 0)
                    {
                        //到第一个按钮时允许跳到上一组
                        GroupIndexAdd(false);
                        currBtnIndex = groups[currGroupIndex].btns.Length - 1;
                    }
                    else
                    {
                        currBtnIndex = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 当组索引或按钮索引更新后执行此函数更新状态
        /// </summary>
        public void OnBtnIndexUpdate()
        {
            int groupsCount = groups.Length;
            for (int gIndex = 0; gIndex < groupsCount; gIndex++)
            {
                Group group = groups[gIndex];
                if (gIndex == currGroupIndex)
                {
                    for (int bIndex = 0; bIndex < group.btns.Length; bIndex++)
                    {
                        group.btns[bIndex].OnSelected(bIndex == currBtnIndex);
                    }
                }
                else
                {
                    for (int bIndex = 0; bIndex < group.btns.Length; bIndex++)
                    {
                        group.btns[bIndex].OnSelected(false);
                    }
                }
            }
        }
    }
}