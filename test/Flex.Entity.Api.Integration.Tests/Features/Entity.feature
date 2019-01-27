Feature: Entity
	In order to manage entities in flex
	As an OE user
	I want to be be able to create, delete and view entities types.

# Legends (Annotations)
# PAT - ProductAcceptanceTest
# NAT - NegativeAcceptanceTest
# TAT - TechnicalAcceptanceTest


@PAT
Scenario: E01 Create Entity - Create a new entity 
	Given a request to create a new entity with entity type 'Client'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a POST request with the body '{ name : "Sainsbury", typePrefix : "c", service_parent : null, asset_parent : "OE1"}'
	Then the Api returns with response code '201'
	And the response contains the newly created entity

@PAT
Scenario: E02 Create Entity - Create a new entity 
	Given a request to create a new entity with entity type 'Client'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a POST request with the body '{ name : "Tesco", typePrefix : "c", service_parent : null, asset_parent : "OE1"}'
	Then the Api returns with response code '201'
	And the response contains the newly created entity

@PAT
Scenario: E03 Create Entity - Create a new descendant entity 
	Given a request to create a new entity with entity type 'Site'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a POST request with the body '{ name : "Northcheam", typePrefix : "s", service_parent : null, asset_parent : "c1"}'
	Then the Api returns with response code '201'
	And the response contains the newly created entity

@PAT
Scenario: E04 Create Entity - Create a new service entity 
	Given a request to create a new entity with entity type 'Grid'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a POST request with the body '{ name : "National Grid", typePrefix : "g", service_parent : "OE1", asset_parent : null}'
	Then the Api returns with response code '201'
	And the response contains the newly created entity

@NAT
Scenario: E05 Create Entity - Fail to create a new entity 
	Given a request to create a new entity with entity type 'Site'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a POST request with the body '{ name : "Northcheam", typePrefix : "s", service_parent : "c1", asset_parent : null}'
	Then the Api returns with response code '400'
	And the response contains the creation failure error message

@PAT
Scenario: E06 Update Entity - Update an existing entity
	Given a request to update an entity with an entity code 'g1'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a PATCH request with the body '{ name : "National Grid 2" }'
	Then the Api returns with response code '200'
	And the response contains the update success message

@PAT
Scenario: E07 Update Entity - Reparent an existing entity
	Given a request to update an entity with an entity code 's1'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a PATCH request with the body '{ asset_parent : "c2" }'
	Then the Api returns with response code '200'
	And the response contains the update success message

@NAT
Scenario: E08 Update Entity - Fail to reparent an existing entity
	Given a request to update an entity with an entity code 's1'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a PATCH request with the body '{ service_parent : "g1" }'
	Then the Api returns with response code '400'
	And the response contains the update failure error message

@NAT
Scenario: E09 Update Entity - Fail to update a non existing entity
	Given a request to update an entity with an entity code 's12323'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a PATCH request with the body '{ service_parent : "g1" }'
	Then the Api returns with response code '404'
	And the response contains the update failure error message


@PAT
Scenario: E10 Get Entity - Get entity by its code
	Given a request to get an entity with an entity code 'g2'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the get response contains the entity details

@PAT
Scenario: E11 Get Entity - Get entity by its code at a point in time
	Given a request to get an entity with an entity code 'g2'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	And at a point in time '?at={now}'
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the get response contains the entity details

@PAT
Scenario: E12 Get Entity - Get entity children by its code
	Given a request to get an entity with an entity code 'oe1'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	And a command to retrieve its children 'children/asset'
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the get response contains the entity details and a collection of child entities


@PAT
Scenario: E14 Get Entity - Get entities by search filter option name
	Given a request to get entities filtered by '?name=Northcheam'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the get response contains a collection of entities match the filter

@PAT
Scenario: E15 Get Entity - Get entities by search filter option top
	Given a request to get entities filtered by '?top=10'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the get response contains a collection of entities match the filter

@PAT
Scenario: E16 Get Entity - Get entities by search filter option skip
	Given a request to get entities filtered by '?skip=2'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the get response contains a collection of entities match the filter

@PAT
Scenario: E17 Get Entity - Get entities by search filter option service_descendant
	Given a request to get entities filtered by '?service_descendant=g1'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the get response contains a collection of entities match the filter

@PAT
Scenario: E18 Get Entity - Get entities by search filter option asset_descendant
	Given a request to get entities filtered by '?asset_descendant=c1'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the get response contains a collection of entities match the filter

@PAT
Scenario: E19 Get Entity - Get entities by search filter option service_child
	Given a request to get entities filtered by '?service_child=g1'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the get response contains a collection of entities match the filter

@PAT
Scenario: E20 Get Entity - Get entities by search filter option asset_child
	Given a request to get entities filtered by '?asset_child=c1'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the get response contains a collection of entities match the filter

@PAT
Scenario: E21 Get Entity - Get entities by search filter option has_tag
	Given a request to get entities filtered by '?has_tag=dummy'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the get response contains a collection of entities match the filter

@PAT
Scenario: E22 Get Entity - Get entities by search filter option matches_tag
	Given a request to get entities filtered by '?matches_tag=dummy:32'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the get response contains a collection of entities match the filter

@PAT
Scenario: E30 Delete Entity - Delete a leaf entity
	Given a request to delete an entity with an entity code 'g2'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a DELETE request
	Then the Api returns with response code '200'
	And the response is empty

@PAT
Scenario: E31 Delete Entity - Delete a non-leaf entity
	Given a request to delete an entity with an entity code 'c3'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	And a flag indicating to delete all descendants '?delete_all_descendants=true'
	When the client makes a DELETE request
	Then the Api returns with response code '200'
	And the response is empty

@PAT
Scenario: E32 Delete Entity - Delete a non-leaf entity
	Given a request to delete an entity with an entity code 'c4'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	And a flag indicating to delete all descendants '?delete_all_descendants=true'
	When the client makes a DELETE request
	Then the Api returns with response code '200'
	And the response is empty

@NAT
Scenario: E33 Delete Entity - Fail to delete a non existing entity
	Given a request to delete an entity with an entity code 'c123432'
		| Name        | Value             |
		| RelativeUrl | /entities         |
		| Accept      | application/json; |
	When the client makes a DELETE request
	Then the Api returns with response code '404'
	And the response is empty
