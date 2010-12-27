<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Home.ViewModels.RulesViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<h4><%= Model.SiteName %> 8-Ball Rules of Play</h4>
<p>Revised: 4 January 2010</p>
<p>All players participating in the <%= Model.SiteName %> agree to abide by the modified 
  <a href="http://www.bca-pool.com/"> BCA (Billiards Congress of America)</a> 
  <a href="http://www.billiards.com/article/official-bca-8-ball-rules">General Rules</a>
  as described below:</p>
<ol class="rules">
  <li>
    <span class="title">Object of the game</span>
    <p>Eight-Ball is a <strong>call shot</strong> game played with a cue ball and 15 object balls, numbered 1 
    through 15. One player must pocket balls of the group numbered 1 through 7 (solid colors), 
    while the other player has 9 through 15 (stripes). The player pocketing either group first, 
    and then legally pocketing the 8-ball wins the game.</p>
  </li>
  <li>
    <span class="title">Call shot</span>
    <p>In Call Shot, obvious balls and pockets do not have to be indicated; however, it is the 
    opponent’s right to ask which ball and pocket if he is unsure of the shot. Bank shots and 
    combination shots are not considered obvious, and care should be taken in calling both the 
    object ball and the intended pocket. When calling the shot, it is never necessary to indicate 
    details such as the number of cushions, banks, kisses, caroms, etc.</p>
    <p>Any balls pocketed on a foul shot remain pocketed, regardless of whether they belong to 
    the shooter or the opponent.</p>
    <p>The opening break is not a called shot. Any player performing a break shot in 8-Ball may 
    continue to shoot so long as any object ball is legally pocketed on the break.</p>
  </li>
  <li>
    <span class="title">Racking the balls</span>
    <p>The balls are racked in a triangle at the foot of the table with the 8-ball in the center 
    of the triangle, the first ball of the rack on the foot spot, a stripe ball in one corner of 
    the rack and a solid ball in the other corner.</p>
    <p><%= Html.ContentImage("rack.jpg", "rack") %></p>
  </li>
  <li>
    <span class="title">Order of break</span>
    <p>It is up to the two opponents to decide whether to lag or toss a coin. The winner has the 
    option to break. The winner of each game breaks in the next.</p>
  </li>
  <li>
    <span class="title">Legal break shot</span>
    <p>To execute a legal break, the breaker (with the cue ball behind the head string) must either
    pocket a ball or drive at least four numbered balls to the rail. The table remains open after 
    the break.</p>
    <p>When the breaker fails to make a legal break, it is a foul, and the incoming player has the 
    option of accepting the table in position and shooting or having the balls re-racked and having 
    the option of shooting the opening break or allowing the offending player to re-break.</p>
  </li>
  <li>
    <span class="title">Scratch on break</span>
    <p>If a player scratches on a break shot, it is considered a foul. All balls pocketed remain 
    pocketed (exception, the 8-ball: see rule 8) and the table remains open. The incoming player 
    has cue ball-in-hand behind the head string and may not shoot an object ball that is behind the 
    head string, unless he first shoots the cue ball past the head string and causes the cue ball 
    to come back behind the head string and hit the object ball.</p>
  </li>
  <li>
    <span class="title">Object balls jumped off table on the break</span>
    <p>If a player jumps an object ball off the table on the break shot, it is considered a foul.
    Any pocketed balls remain pocketed; if a ball other than the cue ball jumped off the table, 
    it is not spotted. The table remains open and the incoming player has the option of accepting 
    the table in position and shooting or taking cue ball-in-hand behind the head string and 
    shooting if the cue ball jumped off the table.</p>
  </li>
  <li>
    <span class="title">8-Ball pocketed on the break</span>
    <p>If the 8-ball is pocketed on the break, breaker may ask for a re-rack or have the 8-ball 
    spotted and continue shooting. The table is considered open. If the breaker scratches while 
    pocketing the 8-ball on the break, the incoming player has the option of a re-rack or having 
    the 8-ball spotted and begin shooting with ball-in-hand behind the head string.</p>
  </li>
  <li>
    <span class="title">Open table</span>
    <p>The table remains open when the choice of groups (stripes or solids) has not yet been 
    determined. When the table is open, it is legal to hit a solid first to make a stripe or 
    vice-versa.</p>
    <p>Note: The table is always open immediately after the break shot whether a ball was 
    pocketed or not. When the table is open, it is legal to hit any solid or stripe first in the 
    process of pocketing the called stripe or solid ball. However, when the table is open and the 
    8-ball is the first ball contacted, it is a foul and no stripe or solid may be scored in 
    favor of the shooter. The shooter loses his turn; the incoming player is awarded cue ball-in-hand; 
    any balls pocketed remain pocketed; and the incoming player addresses the balls with the table 
    still open. On an open table, all illegally pocketed balls remain pocketed.</p>
  </li>
  <li>
    <span class="title">Choice of group</span>
    <p>The choice of stripes or solids is not determined on the break even if balls are made from 
    only one or both groups, because the table is always open immediately after the break shot. 
    The choice of group is determined only when a player legally pockets a called object ball 
    after the break shot.</p>
  </li>
  <li>
    <span class="title">Legal shot</span>
    <p>On all shots (except on the break and when the table is open), the shooter must hit one of 
    his group of balls first and pocket a numbered ball or cause the cue ball or any numbered ball to 
    contact a rail.</p>
    <p>Note: It is permissible for the shooter to bank the cue ball off a rail before contacting the 
    object ball. However, after contact with the object ball, an object ball must be pocketed, 
    or the cue ball or any numbered ball must contact a rail. Failure to meet these requirements is a 
    foul and results in ball-in-hand for his opponent.</p>
    <p>Note: Care should be taken so that a player does not take an excessively long time before 
    committing to a shot. Although it is understandable that some shots are harder than others a 
    1-minute time limit should not be exceeded, on average, per shot.</p>
  </li>
  <li>
    <span class="title">Timeouts</span>
    <p>One 2-minute timeout is allowed per game for a player to consult with his/her partner ONLY.</p>
  </li>
  <li>
    <span class="title">Safety/Defensive shot</span>
    <p>For tactical reasons, a player may choose to pocket an obvious object ball and also discontinue
     a turn at the table by declaring "safety" in advance. A safety shot is defined as a legal shot. 
     If the shooting player intends to play safe by pocketing an obvious object ball, then prior to the 
     shot, the shooter must declare a "safety" to the opponent. It is the shooter's responsibility to 
     make the opponent aware of the intended safety shot. If this is not done, and one of the shooter's 
     object balls is pocketed, the shooter will be required to shoot again. Any ball pocketed on a 
     safety shot remains pocketed.</p>
  </li>
  <li>
    <span class="title">Winning the game</span>
    <p>A player is entitled to continue shooting until failing to legally pocket a ball of his group. 
    After a player has legally pocketed all of his group of balls, he shoots to pocket the 8-ball.</p>
  </li>
  <li>
    <span class="title">Foul penalty</span>
    <p>When a foul shot occurs, the opposing player gets cue ball-in-hand. This 
    means that the player can place the cue ball anywhere on the table (does not 
    have to be behind the headstring except on opening break). This rule prevents 
    a player from making intentional fouls which would put an opponent at a 
    disadvantage. With "cue ball in hand" the player may use a hand or any part 
    of a cue (including the tip) to position the cue ball. However, any forward 
    stroke motion by contacting the cue ball with the tip of the cue will be 
    considered a foul.</p>
  </li>
  <li>
    <span class="title">Combination of shots</span>
    <p>Combination shots are allowed however the 8-ball can’t be used as a first 
    ball in the combination unless it is the shooter’s only remaining legal object 
    ball on the table. Otherwise, should such contact occur on the 8-ball, it is 
    a foul.</p>
  </li>
  <li>
    <span class="title">Illegally pocketed balls</span>
    <p>An object ball is considered to be illegally pocketed when one of the following
    occurs:
    <ol>
      <li>The object ball is pocketed on the same shot a foul is committed</li>
      <li>The called ball did not go in the designated pocket</li>
      <li>A safety is called prior to the shot</li>
    </ol>
    </p>
    <p>Illegally pocketed balls remain pocketed and are scored in favor of the shooter 
    controlling that specific group of balls, solids or stripes.</p>
  </li>
  <li>
    <span class="title">Object balls jumped off the table</span>
    <p>If any object ball is jumped off the table it is a foul and loss of turn 
    unless it is the 8-ball, which is a loss of game. Any jumped object balls are 
    not re-spotted.</p>
  </li>
  <li>
    <span class="title">Jump and massé shot foul</span>
    <p>While "cue ball fouls only" is the rule of play when a match is not presided 
    over by a referee, a player should be aware that it will be considered a cue 
    ball foul if during an attempt to jump, curve or massé the cue ball over or 
    around an impeding numbered ball that is not a legal object ball, the impeding 
    ball moves (regardless of whether it was moved by a hand, cue stick 
    follow-through or bridge).</p>
    <p>Note: Jump shots must be performed by hitting the cue ball into the table's 
    surface so that it rebounds from the cloth. Scooping under the cue ball to 
    fling it into the air is deemed a foul.</p>
    <p>
      <%= Html.ContentImage("jump1.jpg", "Jump") %>
      <%= Html.ContentImage("jump2.jpg", "Jump") %>
    </p>
  </li>
  <li>
    <span class="title">Playing the 8-ball</span>
    <p>When the 8-ball is the legal object ball, a scratch or foul is not loss 
    of game if the 8-ball is not pocketed or jumped from the table. The incoming 
    player has cue ball-in-hand.</p>
    <p>Note: A combination shot can never be used to legally pocket the 8-ball, 
    except when the 8-ball is the first ball contacted in the shot sequence.</p>
  </li>
  <li>
    <span class="title">Loss of game</span>
    <p>A player loses the game by committing any of the following infractions:
    <ol>
      <li>Fouls when pocketing the 8-ball (exception: see 8-Ball Pocketed On The Break)</li>
      <li>Pockets the 8-ball on the same stroke as the last of his group of balls</li>
      <li>Jumps the 8-ball off the table at any time</li>
      <li>Pockets the 8-ball in a pocket other than the one designated</li>
      <li>Pockets the 8-ball when it is not the legal object ball</li>
    </ol>
    </p>
    <p>Note: All infractions must be called before another shot is taken, or else 
    it will be deemed that no infraction occurred.</p>
    <p>Note: Three consecutive fouls by one player is not considered loss of game.</p>
  </li>
  <li>
    <span class="title">Stalemated game</span>
    <p>If, after 3 consecutive turns at the table by each player (6 turns total), 
    both players judge that attempting to pocket or move an object ball will result 
    in loss of game, the balls will be re-racked with the original breaker of the 
    stalemated game breaking again. The stalemate rule may be applied regardless 
    of the number of balls on the table.</p>
  </li>
</ol>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">Rules</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
