# Patchnotes

## Version 1.9.3.0

### c-Time API improvements
This update mostly contains improvements and changes on how the c-Time API is called.  
These changes are completely transparent for the users, everything should still behave as expected.  
If you have a half day off in the afternoon, and this half day is entered into c-Time in advance, you should now be able to still use this App to checkin and -out.

## Version 1.9.2.0

### c-Time on-premises support
This app now supports c-Time on-premises, hosted on your own server.  
Just enter the URL for your c-Time installation in the settings and you're ready to use this app with your own version of c-Time.

## Version 1.9.1.0

### Fluent Design
I added some nice animations all over the place.  
And if you have the Fall Creators Update for Windows 10, there is also a new navigation.

### Smaller adjustments
c-time was renamed to c-Time, the capital T makes all the difference.  
Other than that, there were just smaller adjustments and error corrections.

## Version 1.9.0.0

### Live-Tile
This app now supports the windows 10 live-tiles.  
This means, if you pinned this app to your start-screen, then you can see some c-Time data on the tile itself.  
For example how long you have worked today already, or when you checked-in or -out.

### Notifications when someone checks in
This is especially useful in the morning or after break.  
If you wait for a coworker, then you receive a nice notification once he checks in.

### Improved communication with c-Time
There are to tiny improvements how we call the c-Time API.  
First, there is now a little cache in place. So if you navigate around very much, the performance of this App is greatly improved.  
Second, if there is an error calling the c-Time API, we will retry the request. This helps a lot on bad connections.

### Bug in statistics
An error would occur in the statistics, if in the selected timespan no times were found.  
This is fixed now.

### Improved "My times"
You can now see what kind of time it is, vacation or home-office for example.  
There is a warning now for workdays without any times.

## Version 1.8.2.0

### Bug with todays workend
There was an error in the last version which lead to the todays workend not working correctly.  
That is fixed again in this version.  

## Version 1.8.1.0

### Breaktime in the overview
The overview now has, just like the overtime, a timer for your current breaktime.  
You can now see how long your current break is, and when it should end.  

### Attendance list is working again
There was an error in the last version.  
In the attendance list everyone was shown as Away.  
This is fixed now. The attendance list should work as expected again.  

### New setting for workdays
There is a new setting where you can specify which weekdays are your workdays.  
This is used in the overtime statistics. 

## Version 1.8.0.0

### Extended attendance list
The attendance list now shows how someone is checked in or checked out.  
You can see now, if someone is checked-in in home-office.  
Also, theres a new search feature in the attendance list.  

### About and settings
The about area was completely reworked as well.  
For example, you can now give feedback using the feedback hub.  
In the settings is a new setting for analytics.  

### New diagrams
The diagrams in the statistics have are completely reworked.  
They look much more modern now, and have a couple of new features.  

## Version 1.7.1.0

### Crash at startup
This app was crashing on some systems. It should be fixed now.  

### See patchnotes again
In the area "About" you can see these patchnotes again.  

## Version 1.7.0.0

### Custom employee groups!
To get a better overview in the attendance list, you can now create your own groups of employees.  

### Start- and enddate
In your times and the statistics the start- and enddate will now be remembered.  

### Share!
Some information from this app can now be shared with other apps.  
You can use this to send your times per email for example.  

## Version 1.6.1.0

### Improved attendance list!
In the attendance list you can now click on an employee to see more infos about him.  
For example, you can send him an email, call him, or add him to your contacts.  

### General improvements!
Right after you have checked-in it was possible for the timer to run backwards - not anymore.  
In "Your times" you can now see the selected date range in the header, just like you can in the statistics area.  
The system accent color is now used all over the app.  

### Improved statistics!
There is a new statistic for todays workende without overtime.  
Also, the positioning especially with large names has been improved.  
In the last version has been a little bug, which is why the buttons for "This month", "Last month", and "Last 7 days" did not work. That is fixed.  

## Version 1.6.0.0

### Stamp with geolocation!
c-Time has had this feature for a while now, but finally this app supports geolocation as well.  
That means, if you check-in or -out, your position will be send to c-Time.  
the overview you will see a icon for that.  

### Improvements in the overview
If you have worked long enough today, your overtime will be in a separate counter.  
If you click on your time, you will switch automatically to "My times" for today.  

### Update to use the new c-Time API!
This app now uses the new c-Time API.  
That means the login is more secure now, your password will not be sent to the server.  
Also, the attendance list will load much faster.  

### Reworked settings!
In the new settings you can define how long a work day and the break is.  
These values are used in the statistics and overview for the overtime.  
Also, you can set the theme of this app.  

### Other improvements!
In the statistics, you can now decide whether today is included in the statistics.