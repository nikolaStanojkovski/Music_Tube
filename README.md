# Music Tube
-----------------------------------------------------------------------------------
## Web Application made by Nikola Stanojkovski
-----------------------------------------------------------------------------------
<b>MusicTube</b> is an web application for listening to music and watching music videos. 
<br />
<br />
The framework used to implement the system is <b>.NET Core</b> as a back-end tool, and <b>Server-Side Razor</b> as the front-end tool. <b>HTML, CSS</b> and <b>Bootstrap</b> are used for the layout and the design of the pages, while <b>JavaScript</b> and <b>JQuery</b> are used for handling user's activities, creating the appropriate animations and sending requests to the server that enable interactive design.
The application uses <b>SQL Server</b> as a database server, <b>Onion</b> architecture (<b>Domain, Repository, Service, Web</b> layers) as a main architectural pattern, and, <b>C#</b> as a main programming language.
<br/>
<br/>
The application has the following functionalities:
<br/>
- <b>User Registration / Login</b> with two types/roles of users
  -  <b>Creators</b>, artists, which are the types of users who upload and change the content (videos and songs)
  -  <b>Listeners</b>, which are the types of users who listen to content
- <b>List view</b> of all available <b>songs</b>
- <b>List view</b> of all available <b>videos</b>
- <b>List view</b> of all available <b>albums</b>
- <b>List view</b> of all available <b>artists</b>
- <b><i>CRUD</i> Operations</b> for two types of <b>media</b>
  - <b>Songs</b>, which users can create and listen to in <i>.wav</i> or <i>.mp3</i> formats
  - <b>Videos</b>, which users can create and listen to in <i>.mp4</i> format
- <b>Filtering</b> the <b>media</b>
  - By genre, song, name, description and label for <b>videos</b>
  - By genre, name, description and label for <b>songs</b>
- <b>Searching</b> for <b>media</b> by any text which is contained in name, description, label or genre
- <b>Sorting</b> the <b>media</b> by rating or popularity
- <b>Filtering, sorting, searching</b> and <b>pagination</b> the artists by any property
- <b>Filtering, sorting, searching</b> and <b>pagination</b> the albums by any property
- <b>Premium subscription plan</b> option for <b>creators</b> who want to <b>upload full albums</b> (songs can be added after the creation of the album):
  - <b>Bronze subscription plan</b> which offers 2 published albums for 10$
  - <b>Silver subscription plan</b> which offers 5 published albums for 20$
  - <b>Gold subscription plan</b> which offers 15 published albums for 50$
  - <b>Diamond subscription plan</b> which offers unlimited published albums for 20$
- <b>Newsletters subscription</b> for users who want to recieve <b>Email notifications</b> about their <b>favourite arist</b> uploading a <b>song</b> or an <b>album</b>
- <b><i>Excel</i> export</b> for <b>artists</b>, unfiltered or filtered by genre
- <b><i>Excel</i> export</b> for <b>albums</b>, unfiltered or filtered by genre
- <b>Leaving feedback</b> including comments and likes for any authenticated user
- <b>Leaving reviews</b> inclduing summary, description and rating for any listener
