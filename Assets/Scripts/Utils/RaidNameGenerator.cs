using UnityEngine;

namespace Utils {
    // this clas will help generate random raid names using a list of adjectives and nouns as also using a list of vowels and consonants
    class RaidNameGenerator {
        private static readonly string[] _adjectives = new string[] {
            "Adorable", "Adventurous", "Aggressive", "Agreeable", "Alert", "Alive", "Amused", "Angry", "Annoyed", "Annoying", "Anxious", "Arrogant",
            "Ashamed", "Attractive", "Average", "Awful", "Bad", "Beautiful", "Better", "Bewildered", "Black", "Bloody", "Blue", "Blue-eyed", "Blushing",
            "Bored", "Brainy", "Brave", "Breakable", "Bright", "Busy", "Calm", "Careful", "Cautious", "Charming", "Cheerful", "Clean", "Clear", "Clever",
            "Cloudy", "Clumsy", "Colorful", "Combative", "Comfortable", "Concerned", "Condemned", "Confused", "Cooperative", "Courageous", "Crazy", "Creepy",
            "Crowded", "Cruel", "Curious", "Cute", "Dangerous", "Dark", "Dead", "Defeated", "Defiant", "Delightful", "Depressed", "Determined", "Different",
            "Difficult", "Disgusted", "Distinct", "Disturbed", "Dizzy", "Doubtful", "Drab", "Dull", "Eager", "Easy", "Elated", "Elegant", "Embarrassed",
            "Enchanting", "Encouraging", "Energetic", "Enthusiastic", "Envious", "Evil", "Excited", "Expensive", "Exuberant", "Fair", "Faithful", "Famous",
            "Fancy", "Fantastic", "Fierce", "Filthy", "Fine", "Foolish", "Fragile", "Frail", "Frantic", "Friendly", "Frightened", "Funny", "Gentle", "Gifted",
            "Glamorous", "Gleaming", "Glorious", "Good", "Gorgeous", "Graceful", "Grieving", "Grotesque", "Grumpy", "Handsome", "Happy", "Healthy", "Helpful",
            "Helpless", "Hilarious", "Homeless", "Homely", "Horrible", "Hungry", "Hurt", "Ill", "Important", "Impossible", "Inexpensive", "Innocent",
            "Inquisitive", "Itchy", "Jealous", "Jittery", "Jolly", "Joyous", "Juicy", "Kind", "Lazy", "Light", "Lively", "Lonely", "Long", "Lovely", "Lucky",
            "Magnificent", "Misty", "Modern", "Motionless", "Muddy", "Mushy", "Mysterious", "Nasty", "Naughty", "Nervous", "Nice", "Nutty", "Obedient",
            "Obnoxious", "Odd", "Old-fashioned", "Open", "Outrageous", "Outstanding", "Panicky", "Perfect", "Plain", "Pleasant", "Poised", "Poor", "Powerful",
            "Precious", "Prickly", "Proud", "Puzzled", "Quaint", "Real", "Relieved", "Repulsive", "Rich", "Scary", "Selfish", "Shiny", "Shy", "Silly",
            "Sleepy", "Smiling", "Smoggy", "Sore", "Sparkling", "Splendid", "Spotless", "Stormy", "Strange", "Stupid", "Successful", "Super", "Talented",
            "Tame", "Tasty", "Tender", "Tense", "Terrible", "Thankful", "Thoughtful", "Thoughtless", "Tired", "Tough", "Troubled", "Ugliest", "Ugly",
            "Uninterested", "Unsightly", "Unusual", "Upset", "Uptight", "Vast", "Victorious", "Vivacious", "Wandering", "Weary", "Wicked", "Wide-eyed",
            "Wild", "Witty", "Worried", "Worrisome", "Wrong", "Zany", "Zealous"
        };

        private static readonly char[] _vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
        private static readonly char[] _consonants = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k','l', 'm', 'n',
        'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };

        public static string GenerateNickname() {
            var adjective = _adjectives[Random.Range(0, _adjectives.Length)];
            var noun = GenerateNoun();
            return $"{adjective} {noun}";
        }

        private static string GenerateNoun() {
            var length = Random.Range(3, 8);
            var word = "";
            for (var i = 0; i < length; i++) {
                if (i % 2 == 0) {
                    word += _consonants[Random.Range(0, _consonants.Length)];
                }
                else {
                    word += _vowels[Random.Range(0, _vowels.Length)];
                }
            }
            return word;
        }

    }
}