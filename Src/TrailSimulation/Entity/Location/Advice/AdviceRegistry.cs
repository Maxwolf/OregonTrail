// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdviceRegistry.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Entity
{
    /// <summary>
    ///     References all the advice that people will offer up to the player when they talk to people. The difference pieces
    ///     of advice are broken up into chunks based the type of location and status on the trail they might represent. These
    ///     are suggestions and definitely not guidelines the text could say anything, all I did was want to re-create it
    ///     faithfully as it existed before.
    /// </summary>
    public static class AdviceRegistry
    {
        /// <summary>
        /// Defines the default advice that will be shown on the talk to people form if no advice is loaded.
        /// </summary>
        public const string DEFAULTADVICE = "[DEFAULT ADVICE]";

        /// <summary>
        ///     Advice intended for new players that are starting out on the trail, normally this advice is used on the first
        ///     location.
        /// </summary>
        public static Advice[] Tutorial
        {
            get
            {
                var startingAdvice = new[]
                {
                    new Advice("A trader named Jimmy", 
                        "Better take extra sets of clothing. Trade 'em to Indians for fresh vegetables, fish, or meat. It's well worth hiring an Indian guide at river crossings. Expect to pay them! They're sharp traders, not easily cheated."), 
                    new Advice("A traveler", 
                        "Did you read the Missouri Republican today? --Says some folk start for Oregon without carrying spare parts, not even an extra wagon axle. Must think they grow on trees! Hope they're lucky enough to find an abandoned wagon."), 
                    new Advice("A town resident", 
                        "Some folks seem to think that two oxen are enough to get them to Oregon! Two oxen can barely move a fully loaded wagon, and if one of them gets sick or dies, you won't be going anywhere. I wouldn't go overland with less than six."), 
                    new Advice("Aunt Rebeka", 
                        "With the crowds of people waiting to get on the ferry, we could be stranded here for days! Hope there's enough graze for all those animals -- not many people carry feed! I'd rather wait, though, than cross in a rickety wagon boat!")
                };
                return startingAdvice;
            }
        }

        /// <summary>
        ///     Advice intended to be used at the first river crossing the player vehicle encounters.
        /// </summary>
        public static Advice[] River
        {
            get
            {
                var advice = new[]
                {
                    new Advice("Sam Hendricks", 
                        "Can't afford to take a ferry. We're making our wagon into a boat. We'll turn it over, caulk the bottom and sides with pitch, and use it to float our goods across. Have to swim the animals. Hope it doesn't rain -- the river's high enough!"), 
                    new Advice("A ferry operator", 
                        "Don't try to ford any river deeper than the wagon bed -- about two and a half feet. You'll swamp your wagon and lose your supplies. You can caulk the wagon bed and float it -- or be smart and hire me to take your wagon on my ferry!"), 
                    new Advice("A party leader heading east", 
                        "We've had enough! Pesky flies all day and mosquitoes all night! It's either baking sun or oceans of mud -- and sometimes both. Worry over Indians attacking -- haven't seen any yet, but still a worry."), 
                    new Advice("A lady", 
                        "This prairie is mighty pretty with all the wild flowers and tall grasses. But there's too much of it! I miss not having a town nearby. I wonder how many days until I see a town -- a town with real shops, a church, people..."), 
                    new Advice("Big Louie", 
                        "Be careful you don't push those animals too hard! Keep 'em moving but set them a fair pace. Can't keep driving 'em so fast or you'll end up with lame-footed animals. A lame ox is about as good to you as a dead one!"), 
                    new Advice("A fort soldier", 
                        "The trails from the jumping off places -- Independence, St. Joseph, Council Bluffs -- come together at Fort Kearney. This new fort was built by the U.S. Army to protect those bound for California and Oregon."), 
                    new Advice("Big Louie", 
                        "The Platte River valley forms a natural roadway from Fort Kearney to Fort Laramie. Travelers bound for California, Utah, and Oregon all take this road. Could be the easiest stretch of the whole trip. Should see antelope and plenty of buffalo."), 
                    new Advice("A Fort Kearney scout", 
                        "The game is still plentiful along here, but gettin' harder to find. With so many overlanders, I don't expect it to last more'n a few years. Folks shoot the game for sport, take a small piece, and let the rest rot in the sun."), 
                    new Advice("Aunt Rebecca", 
                        "I hear terrible stories about wagon parties running out of food before Oregon -- the whole party starving to death. We must check our supplies often; we might not get there as soon as we think. Always plan for the worst, I say.")
                };
                return advice;
            }
        }

        /// <summary>
        ///     Advice that is given out at any given landmark, the information here is non-specific and will make sense at any
        ///     landmark. Most of the advice is people warning about things they heard about other people ahead of them.
        /// </summary>
        public static Advice[] Landmark
        {
            get
            {
                var advice = new[]
                {
                    new Advice("Celinda Hines", 
                        "Chimney Rock by moonlight is awfully sublime. Many Indians came to our wagon with fish to exchange for clothing. We bought a number. They understand 'swap' and 'no swap.' Seem most anxious to get shirts and socks."), 
                    new Advice("Alonzo Delano", 
                        "About noon yesterday we came in sight of Chimney Rock looming up in the distance like the lofty tower of some town. We did not tire gazing on it. It was about 20 miles from us, and stayed in sight 'til we reached it today."), 
                    new Advice("A Sioux hunter", 
                        "The Pawnee are the mortal enemies of the Sioux. I would not hesitate to kill any Pawnee I met. But I have never killed a white man. All I ask from the white man is to leave me alone, and to leave my buffalo alone."), 
                    new Advice("A woman traveler", 
                        "Be warned, stranger. Don't dig a water hole! Drink only river water. Salty as the Platte River is -- it's better than the cholera. We buried my husband last week. Could use some help with this harness, if you can spare the time."), 
                    new Advice("A mountain man", 
                        "These greenhorns heading across the Rockies know nothing about surviving in the mountains. It gets awful cold up there, even in summer. Many a traveler crossing the mountains too late in the year has gotten snowbound and died!"), 
                    new Advice("A young boymaner", 
                        "I carved my name way up the side of Independence Rock, near the top. There are hundreds of names up there! The oldest ones were carved by mountain men and fur trappers -- famous names like Fremont, Bonneville, and DeSmet!"), 
                    new Advice("Aunt Rebecca", 
                        "No butter or cheese or fresh fruit since Fort Laramie! Bless me, but I'd rather have my larder full of food back East than have our names carved on that rock! Well, tis a sight more cheery than all the graves we passed."), 
                    new Advice("Big Louiecca", 
                        "Goodbye Platte River! Goodbye sand hills and white buffalo skulls! Now we climb the Sweetwater valley to cross the Continental Divide at South Pass. Once across the Rockies, we'll make a steep descent into the Green River valley."), 
                    new Advice("A Mormon traveler", 
                        "My family and I travel with 40 other families to the valley of the Great Salt Lake to seek religious freedom. Back east, Mormons are persecuted. In Utah, we'll join together to build a new community, changing desert into farm land."), 
                    new Advice("An Arapaho Indian", 
                        "When the white man first crossed our lands their wagons were few. Now they crowd the trail in great numbers. The land is overgrazed with their many animals. Do any white men still live in the East? My people talk of moving."), 
                    new Advice("A young girl", 
                        "My father is very sick and we are resting here until he gets better. We have been pushing too hard and our health has suffered. When my father is able to travel again, we will go at a slower pacet? My people talk of moving."), 
                    new Advice("A tired-looking woman", 
                        "One child drowned in a swollen creek east of Fort Laramie. My husband died of typhoid near Independence Rock. Now I travel alone with my five children. The eldest, Caleb, is eleven. I fear he'll be a man before we reach Oregon.")
                };
                return advice;
            }
        }

        /// <summary>
        ///     Statements that will generally be said around areas that are more civilized along the route. There is a fair
        ///     mixture of different kinds of people experiencing different types of problems from beginning to end of the trail.
        /// </summary>
        public static Advice[] Settlement
        {
            get
            {
                var advice = new[]
                {
                    new Advice("A trader", 
                        "This fort was built by Jim Bridger. Jim was a mountain man before he put in this blacksmith shop and small store to supply the overlanders. Does a big trade in horses, Jim and his partner, Vasquez."), 
                    new Advice("Aunt Rebecca", 
                        "We should've taken the Sublette Cutoff! Not enough at this fort worth the time it took to get here. And the outrageous prices! Food's not fit to eat, much less pay for. Some folks'd sell the clothes off our backs if we'd let them!"), 
                    new Advice("A Shoshoni Indian", 
                        "When wagons first started coming through here, we did not mind. We even found it good to trade game and fish with the travelers and help them cross the rivers. Now there are too many white men and too little land for grazing."), 
                    new Advice("Big Louie", 
                        "Five dollars to ferry us over the Green River? Those ferrymen'll make a hundred dollars before breakfast! We'll keep down river until we find a place to ford our wagon and animals. What little money we have left, we'll keep!"), 
                    new Advice("A young boy", 
                        "My family didn't buy enough food in Independence. We have been eating very small rations since Fort Laramie. Because of that our health is poor. My sister has mountain fever, so we're stopped here for a while."), 
                    new Advice("Miles Hendrick", 
                        "I've heard it said that there are many cutoffs to take to shorten the journey -- that by taking all the shortcuts, you can save many days on the trail. And why not? Saving time and provisions is worth the risk!"), 
                    new Advice("Celinda Hineskia", 
                        "My, the Soda Springs are so pretty! Seem to spout at regular intervals. Felt good to just rest and not be jostled in the wagon all day. When I get to Oregon, I'll have a soft feather bed and never sleep in a wagon again!"), 
                    new Advice("A young boy", 
                        "My job every day is to find wood for the cook fire. Sometimes it's very hard to find enough, so I store extra pieces in a box under the wagon. On the prairie I gathered buffalo chips to burn when there wasn't any wood."), 
                    new Advice("Miles Hendrick", 
                        "Well, friend, this is where we part. I'm bound for California with an imposing desert to cross. And you -- you've got the Snake River to cross, which I hear is no picnic! Write us, you or the Missus, just as soon as you reach Oregon."), 
                    new Advice("Aunt Rebecca", 
                        "Hear there's mountain sheep around here. Enough water too, but hardly a stick of wood. Thank heaven for Fort Hall! But I'm real sorry to be saying goodbye to cousin Miles and all the folks heading for California."), 
                    new Advice("A fellow traveler", 
                        "Fort Hall is a busy fort! The wide stretches of meadow grass here are just what our tired animals need. As for me, I'll fix up the wagon leaks. Amanda's real anxious to wash all the clothes and linens in one of those clear streams."), 
                    new Advice("A frantic wife", 
                        "It says right here in the Shively guidebook: \"You must hire an Indian to pilot you at the crossings of the Snake river, it being dangerous if not perfectly understood.\" But my husband insists on crossing without a guide!")
                };
                return advice;
            }
        }

        /// <summary>
        ///     Advice that is generally used on locations that are inserted into the trail from forks in the road. The people in
        ///     these comments mostly show anger about toll roads and difficulty with getting over the mountains.
        /// </summary>
        public static Advice[] Mountain
        {
            get
            {
                var advice = new[]
                {
                    new Advice("An overlander", 
                        "Down there between those steep lava gorges, twisting and writhing, is the Snake River. So much water -- and so hard to get to! We've got many miles of desert before Oregon, so be sure to fill your water kegs at the crossing!"), 
                    new Advice("Big Louie", 
                        "See that wild river? That's the Snake. Many a craft's been swamped in her foaming rapids. Her waters travel all the way to Oregon! We'll be crossing her soon, and then again after Fort Boise. Take care at the crossing!"), 
                    new Advice("A trader with 6 mules", 
                        "You'll not get yer wagon over them Blue Mountains, mister. Leave it! Cross yer goods over with pack animals. Get yerself a couple of good mules. Pieces of wagons litter the trail -- left by them folks who don't heed good advice!"), 
                    new Advice("Aunt Rebeccah", 
                        "At every fort along the trail, prices have been higher than at the previous fort! This is outrageous! They're taking advantage of us! If I had the chance to do it again, I'd buy more supplies in Independence."), 
                    new Advice("Jacob Hofstead", 
                        "Every night, even though I ache from the day's toils, my head is filled with dreams of the rich farm land of the Willamette Valley. I will build myself a fine, handsome homestead -- and I'm certain I'll be rich within five years!"), 
                    new Advice("A tired overlander", 
                        "Since crossing the Snake at Fort Boise, it's been just mountains and desert. Dust deeper each day -- six inches at times. No tracks, just clouds of dust. Many cattle choked on the dust after swimming the river, then bled and died.")
                };
                return advice;
            }
        }

        /// <summary>
        ///     Advice from travelers that have been along most of the trail and are now deciding about which trail would be a
        ///     better risk for their vehicle party and offer up advice about their past decisions for replay value.
        /// </summary>
        public static Advice[] Ending
        {
            get
            {
                var advice = new[]
                {
                    new Advice("Marnie Stewart", 
                        "We followed the edge of the desert from Fort Boise to the forbidding wall of the Blue Mountains. The hills were dreadful steep! Locking both wheels and coming down slow, we got down safe. Poor animals! No grass or water for days."), 
                    new Advice("Jacob Hofstead", 
                        "This valley of the Grande Ronde is the most beautiful sight I've seen in months. Water and graze in abundance! And if this valley is so fine, the Willamette must be twice as fine! We'll be sittin' pretty in our new homestead!"), 
                    new Advice("A young mother", 
                        "I've traveled in fear of Indians since our journey began. As of yet we've seen few. Those we met helped us cross rivers or sold us vegetables. Still I fear. I've read grave markers and heard stories of killings in these mountains."), 
                    new Advice("Amy Witherspoon", 
                        "My cousin Catherine was one of six children orphaned and left at Whitman's Mission. Lived with them for three years -- until the massacre last November. She has survived snakebites, stampedes, falls, fights -- not to mention a massacre.."), 
                    new Advice("A Cayuse Indian", 
                        "You ask about the Whitman massacre. I ask you why Doctor Whitman's medicine did not cure my people's children? Many caught the measles from the strangers. Why did the medicine poison our children and cure the children of white people."), 
                    new Advice("A mountain man", 
                        "These last hundred miles to the Willamette Valley are the roughest -- either rafting down the swift and turbulent Columbia River or driving your wagon over the steep Cascade Mountains. Hire an Indian guide if you take the river."), 
                    new Advice("Amy Witherspoon", 
                        "My cousin Lydia engaged passage down the Columbia with Indians -- a canoe with 17 people and luggage! The wind blew so heavy they had to lay by. Near dark, high waves came up over their heads! Finally, they made it to shore safely."), 
                    new Advice("A toll collector", 
                        "I collect the tolls for the Barlow Road -- a bargain at twice the price! Until last year the overlander had no choice -- everyone floated the Columbia. Now with Mr. Barlow's new road, you can drive your wagon right into Oregon City!")
                };
                return advice;
            }
        }
    }
}