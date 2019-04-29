using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BToolkit {
    public abstract class SkillManager {

        Dictionary<string, Skill> skillsPool = new Dictionary<string, Skill>();

        public Skill GetSkill(string skillId) {
            if (skillsPool.ContainsKey(skillId)) {
                return skillsPool[skillId];
            }
            Skill skill = InstanceSkillById(skillId);
            if (skill != null) {
                skillsPool.Add(skillId, skill);
            } else {
                Debug.LogError("找不到ID为 " + skillId + " 的技能,请检查InstanceSkillById()方法。");
            }
            return skill;
        }

        public void Destroy() {
            skillsPool.Clear();
            skillsPool = null;
        }

        public abstract Skill InstanceSkillById(string skillId);
    }
}