using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Use this Function for initialization for EACH CHARACTER
public class InitSkillListOfCharacter : MonoBehaviour {
	public static void Set (GameObject that, string[] skill_names) {
		CharacterController updater = that.GetComponent<CharacterController> ();
		if (updater == null) {
			print ("SetCharacter : can't get updater for " + that.name);
			return;
		} else {
			//print("TOTAL SKILL NUM assigned == " + skill_names.Length + "\nAdd skill on CharacterUpdater");
		}

		List<Skill> slist = updater.skills;

		for (int i = 0; i < skill_names.Length; i++) {
			string sname = skill_names [i];
			Skill  skill = Skill.preset [sname].DeepClone();
            skill.owner         = that.gameObject;
            skill.ownerStat    = that.gameObject.GetComponent<Stat>();
            skill.ownerCommandFilter        = that.gameObject.GetComponent<CommandFilter>();
            skill.ownerCommandFilterWASD    = that.gameObject.GetComponent<CommandFilterWASD>();
            slist.Add(skill);
            //print ("Add skill to CharacterUpdater.skill_list\n sname : " + slist[i].name);
        }
		slist.Sort(new ComplexCommandFirst());
		//print("CharacterUpdater.skill_list.Count = " + slist.Count);


		CommandFilter cmd_filter = that.GetComponent<CommandFilter> ();      // cmd filter에도 스킬 커멘드들 입력해줌 (AI말고 플레이어만 해당)
		if (cmd_filter != null) {
			//print ("fill cmd filter");

			cmd_filter.cmd_list = new CommandFilter.Command[slist.Count];
			CommandFilter.Command[] clist = cmd_filter.cmd_list;

			for (int i = 0; i < slist.Count; i++) {
				clist [i] = Skill.precmd [slist [i].name];
				clist [i].index = i;
			}
		} else {
			// no command filter. (아마도 AI?)
		}

        	
	}

	private class ComplexCommandFirst : IComparer <Skill>{
		public int Compare(Skill a, Skill b){
			CommandFilter.Command ca = Skill.precmd [a.name];	//command
			CommandFilter.Command cb = Skill.precmd [b.name];
			float la = ca.cmd.Length + (ca.check_off ? 0f : 0.01f);	// length compairing. under example is complexcity
			float lb = cb.cmd.Length + (cb.check_off ? 0f : 0.01f); // L+|L-|R+|R-|1+ > L+|L-|R+|1+ > L+|R+|1+ > L+|L-|1+

            if (la > lb) {			return -1;	//reversed -> long cmd first
			} else if (la < lb) {	return +1;	//reversed -> long cmd first
			} else { 				return 00;
			}
		}
	}
}

