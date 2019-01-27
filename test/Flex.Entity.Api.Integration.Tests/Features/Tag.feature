Feature: Tags
	In order to manage Tag value in flex
	As a OE user
	I want to be able to create, update and delete tag type value for a selected entity

# Legends (Annotations)
# PAT - ProductAcceptanceTest
# NAT - NegativeAcceptanceTest
# TAT - TechnicalAcceptanceTest

@PAT
Scenario: T01 Update Entity Tag Values - create an new entity tag value
	Given a request to update an entity tag temp value
		| Name          | Value                                 |
		| RelativeUrl   | /entities/OE1/tags/temp                |
		| Accept        | application/json;						|
	When the client makes a  PUT request with the body '{ value : "32"}'
	Then the Api returns with response code '201'
	And the response value contains '32', key should be 'temp'  and update_at should be correct date

@PAT
Scenario: T02 Update Entity Tag Values - update an new entity tag value
	Given a request to update an entity tag temp value
		| Name          | Value                                 |
		| RelativeUrl   | /entities/OE1/tags/temp               |
		| Accept        | application/json;						|
	When the client makes a  PUT request with the body '{ value : "33"}'
	Then the Api returns with response code '200'
	And the response contains sucess 'true' with error message ''

@PAT
Scenario: T03 GET Entity Tags all Values 
	Given a request for all tag types
		| Name          | Value                                 |
		| RelativeUrl   | /entities/OE1/tags                    |
		| Accept        | application/json;						|
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the response contains a collection of entity OE tag values 

@PAT
Scenario: T04 GET Entity Tags Value
	Given a request for an entity tag value with 'temp'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/OE1/tags                    |
		| Accept        | application/json;						|
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the response contains the requrested entity tag details

@PAT
Scenario: T05 Delete Entity Tag Value - temp
	Given a request to delete an entity value with tag key 'temp'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/OE1/tags/					|
		| Accept        | application/json;						|
	When the client makes a DELETE request
	Then the Api returns with response code '200'
	And the response contains sucess 'true' with error message ''

@PAT
Scenario: T06 Delete Entity Tag Value - which is not exist
	Given a request to delete an entity value with tag key 'X'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/OE1/tags/					|
		| Accept        | application/json;						|
	When the client makes a DELETE request
	Then the Api returns with response code '404'
	And the response contains sucess 'false' with error message 'tag key = X not found.'

@PAT
Scenario: T07 Update Entity Tag Values - create an new entity tag value with null
	Given a request to update an entity tag temp value
		| Name          | Value                                 |
		| RelativeUrl   | /entities/OE1/tags/temperature                |
		| Accept        | application/json;						|
	When the client makes a  PUT request with the body '{ value : null}'
	Then the Api returns with response code '201'
	And the response value contains 'null', key should be 'temperature'  and update_at should be correct date

@PAT
Scenario: T08 Update Entity Tag Values - update an entity tag having null value with not null value
	Given a request to update an entity tag temp value
		| Name          | Value                                 |
		| RelativeUrl   | /entities/OE1/tags/temperature        |
		| Accept        | application/json;						|
	When the client makes a  PUT request with the body '{ value : "35" }'
	Then the Api returns with response code '200'
	And the response contains sucess 'true' with error message ''

@PAT
Scenario: T09 Delete Entity Tag Value - temperature
	Given a request to delete an entity value with tag key 'temperature'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/OE1/tags/					|
		| Accept        | application/json;						|
	When the client makes a DELETE request
	Then the Api returns with response code '200'
	And the response contains sucess 'true' with error message ''





