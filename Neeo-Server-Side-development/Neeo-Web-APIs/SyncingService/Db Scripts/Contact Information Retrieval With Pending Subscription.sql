SELECT #temp.contact,
CASE WHEN (ofUser.username is not null or userRoster.username is not null  or myRoster.jid is not null) 
	THEN ofUser.username 
	ELSE null 
END,
CASE WHEN (ofUser.username = #temp.contact) 
	THEN 1 
	ELSE 0 
END AS [isNeeoUser],
CASE WHEN (userRoster.username is not null and #temp.contact is null )
	THEN 1 
	ELSE 0 
END AS [isPendingSub],
userRoster.sub AS [pendingSubState],
CASE WHEN (myRoster.jid is not null and #temp.contact is not null)
	THEN 1 
	ELSE 0 
END AS [isUserAlreadySub],
myRoster.sub AS [userSubState],
CASE WHEN (myRoster.jid is not null and #temp.contact is null )
	THEN 'remove' 
	ELSE 
		CASE WHEN (myRoster.jid is null and #temp.contact is not null )
			THEN 'add'
			ELSE 'no action'
		END 
END AS [rosterAction]
FROM #temp left join ofUser 
	ON #temp.contact = ofUser.username
FULL join (SELECT ofRoster.username,ofRoster.jid,ofRoster.sub 
		   FROM ofRoster 
		   GROUP BY ofRoster.jid,ofRoster.username,ofRoster.sub
		   HAVING ofRoster.jid = '923458412963@karzantest.net') AS userRoster
	ON #temp.contact = userRoster.username 
FULL join (SELECT ofRoster.username,ofRoster.jid,ofRoster.sub 
		   FROM ofRoster 
		   GROUP BY ofRoster.jid,ofRoster.username,ofRoster.sub
		   HAVING ofRoster.username = '923458412963') AS myRoster
	ON #temp.contact + '@karzantest.net' = myRoster.jid 
ORDER BY #temp.contact DESC