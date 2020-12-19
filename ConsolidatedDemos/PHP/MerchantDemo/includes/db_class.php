<?php

//Include file for database functions, and Db class

/////////////////////////////////////////////////////////////////////////////////////////////
// database class
if(!class_exists('DbClass')) {
class DbClass {
    
    public $connection;
 
    // if $database is set, it is the accountid used to contruct the account database name
    function __construct($host=null, $database=null, $userid=null, $password=null, $env=null) {
      global $myPass;
     
     if($env == 'standalone') {  // just connect to the parameters passed in
         if(!($this->connection = mysqli_connect($host, $userid, $password, $database)))  // 'true' means make sure a new link is made
            $this->showerror();
         return;  
     }
               
     if($host == null) $host = $myPass->pass['user'][0];  // database connection information
     if($database == null) $database = 'groups';    // default database
     if($userid == null)   { $userid = $myPass->pass['user'][2]; $userid2 = $myPass->pass['dba'][2]; }
     if($password == null) { $password = $myPass->pass['user'][3]; $password2 = $myPass->pass['dba'][3]; }
      
     if($database == 'groups') {  // main database name 
         if(!($this->connection = mysqli_connect($host, $userid, $password, $database))) {
            $this->showerror();
         }
      //   if(!mysqli_select_db($database, $this->connection))
       //     $this->showerror();
     }
     // $database will be null for main db 'groups', or have an account_id which defines the account specific db name
     else if($database != null) { // a specific 'account' database - which has documents, folders tables  
         if(!($this->connection = mysqli_connect($host, $userid2, $password2, 'doc_'.$database.'_db')))  // 'true' means make sure a new link is made
            $this->showerror();
       //  if(!mysqli_select_db('doc_'.$database.'_db', $this->connection))
        //    $this->showerror();
     }
    } 	
    
    protected function _getKeyField($tablename) {
        $keyfield == '';
        $query = "SHOW COLUMNS from $tablename";
        $result = @ mysqli_query($this->connection, $query);
        $rowsFound = @ mysqli_num_rows($result);
        
        for($i=0; $i<$rowsFound; $i++) {        
            $row[] = @ mysqli_fetch_assoc($result);
            if($row[$i]['Key'] == 'PRI')
               $keyfield = $row[$i]['Field']; 
        }
        return $keyfield;
    }
    
    public function getKeyValue($tablename, $condition) {
        if($condition == '' || $condition == null)
            return false;
        else {
            if(is_array($condition)) {                                                        
                $firsttime = true;
                foreach($condition as $key => $value) {
                   if($firsttime == false)
                      $sqlcondition .= " AND ";
                   $cleanedValue = mysqli_escape_string($this->connection, $value);
                   $sqlcondition .= $key.' = '."'$cleanedValue' ";
                   $firsttime = false;
                }   
            }
            else {
                $sqlcondition = $condition;
            }
        }
        if($sqlcondition == "") 
           $where = "";
        else
           $where = " WHERE " . $sqlcondition; 

        $keyfield = $this->_getKeyField($tablename);
        $query = "SELECT $keyfield 
                  FROM $tablename
                  $where";

        if (!($result = @ mysqli_query ($this->connection, $query)))
           $this->showerror($query);
        // Find out how many rows there are
        $rowsFound = @ mysqli_num_rows($result);
        if($rowsFound != 1) 
           return false;            
        $value = @ mysqli_fetch_assoc($result); 
           return $value[$keyfield];
    }                  

    
    // error messages
    public function showerror($query='')  {                                          
      if(mysqli_error($this->connection))
        die("MySQL Error: " . $query. ":" . mysqli_error($this->connection));
      else
        die("Could not connect to the DBMS");
    }

    
	public function doQuery($query) {
        if (!($result = @ mysqli_query ($this->connection, $query)))
           $this->showerror($query);
         
        // Find out how many rows there are
        $rowsFound = @ mysqli_num_rows($result);
                    
        for($i=0;$i<$rowsFound;$i++) {        
            $row[] = @ mysqli_fetch_assoc($result); 
        }

        if (!empty($row)) return $row;   // return rows if it a select
        else return true;                // return true otherwise
	}
	
    public function getRowsLike($tablename, $condition, $order, $limit, $like) {
        $returnVal = $this->getRows($tablename, $condition, $order, $limit, $like);
        return $returnVal;
    }
    	
    public function getRows( $tablename, $condition='', $order='', $limit='', $like='') {
        $sqlcondition = '';
        $orderBy = '';             
        $sqllimit = '';

        if($like == 'like')
           $compareType = ' LIKE '; 
        else   
           $compareType = ' = ';    
        
        $row = array();
        // set up condition sql 
        if ($limit != '') 
              $sqllimit = " LIMIT $limit";
        if ($condition != '') {
            if(is_array($condition)) {                                                        
			    $firsttime = true;
			    foreach($condition as $key => $value) {
				   if($firsttime == false)
				      $sqlcondition .= " AND ";
                   $cleanedValue = mysqli_escape_string($this->connection, $value);
                   $sqlcondition .= $key.$compareType."'$cleanedValue' ";  //  '=' or 'like' option
				   $firsttime = false;
				}   
            }
            else {
               $sqlcondition = $condition;
            }
		}
	    if($sqlcondition == "") 
		   $where = "";
		else
		   $where = " WHERE " . $sqlcondition; 
		    
 		if($order != '' && !is_array($order)) {
			$orderBy = " ORDER BY $order ";
		}
        else if(is_array($order) && !empty($order)) {
            $orderBy = ' ORDER BY ';
            $firstTime = true;
        	foreach($order as $field => $theOrder) {
                if($firstTime == true)  { 
                   $orderBy .= " $field $theOrder ";
                   $firstTime = false;
                }
                else {
                   $orderBy .= ", $field $theOrder ";
                }  
            }
        }        
        $query = "SELECT * from $tablename $where $orderBy $sqllimit";
 
       if (!($result = @ mysqli_query ($this->connection, $query)))
         $this->showerror($query);
        // Find out how many rows there are
        $rowsFound = @ mysqli_num_rows($result);
                    
        for($i=0;$i<$rowsFound;$i++) {        
            $row[] = @ mysqli_fetch_assoc($result); 
        }

        if (!empty($row)) return $row;
        else return null;           
    }
  
     // get rows where fieldvalues are in array
     // $condition[field] = array('value1', 'value2', 'value3')
     // $condition[field2] = array(....
     public function getRowsInArray( $tablename, $condition='', $order='', $limit='', $like='') {
        $sqlcondition = '';
        $orderBy = '';             
        $sqllimit = '';

        if($like == 'like')
           $compareType = ' LIKE '; 
        else   
           $compareType = ' = ';    
        
        $row = array();
        // set up condition sql 
        if ($limit != '') 
              $sqllimit = " LIMIT $limit";
        if ($condition != '') {
            if(is_array($condition)) {                                                        
                $firsttime = true;
                foreach($condition as $key => $values) {
                   if($firsttime == false)
                      $sqlcondition .= " AND ";
                   if(is_array($values))  {      // process field IN (list of values)
                      $valueString = implode(',', $values);
                      $cleanedValueString = mysqli_escape_string($this->connection, $valueString);
                      $sqlcondition .= $key.' IN ('.$valueString.')'; 
                   } 
                   else {                   // proces = or LIKE a single value
                      $cleanedValue = mysqli_escape_string($this->connection, $value);
                      $sqlcondition .= $key.$compareType."'$cleanedValue' ";  //  '=' or 'like' option
                   }                       
                   $firsttime = false;
                }   
            }
            else {
               $sqlcondition = $condition;
            }
        }
        if($sqlcondition == "") 
           $where = "";
        else
           $where = " WHERE " . $sqlcondition; 
            
         if($order != '' && !is_array($order)) {
            $orderBy = " ORDER BY $order ";
        }
        else if(is_array($order) && !empty($order)) {
            $orderBy = ' ORDER BY ';
            $firstTime = true;
            foreach($order as $field => $theOrder) {
                if($firstTime == true)  { 
                   $orderBy .= " $field $theOrder ";
                   $firstTime = false;
                }
                else {
                   $orderBy .= ", $field $theOrder ";
                }  
            }
        }        
        $query = "SELECT * from $tablename $where $orderBy $sqllimit";
 
       if (!($result = @ mysqli_query ($this->connection, $query)))
         $this->showerror($query);
        // Find out how many rows there are
        $rowsFound = @ mysqli_num_rows($result);
                    
        for($i=0;$i<$rowsFound;$i++) {        
            $row[] = @ mysqli_fetch_assoc($result); 
        }

        if (!empty($row)) return $row;
        else return null;           
    }
   // -condition is value of key col, or array of single column => value pair
	// expects to return single row
    public function getRow( $tablename, $condition='') {
        
       $row = array();
        // set up condition sql 
       if ($condition != '') {
            if(is_array($condition)) {
                $firstTime = true;
                foreach($condition as $key => $value) {
                   if($firstTime == false)
                      $sqlcondition .= ' AND '; 
                   $cleanedValue = mysqli_escape_string($this->connection, $value);
                   $sqlcondition .= $key.'='."'$cleanedValue'";
                   $firstTime = false;
                }
            }
            else {
                $keyfield = $this->_getKeyField($tablename);
                $cleanedCondition = mysqli_escape_string($this->connection, $condition);
                $sqlcondition = "$keyfield='$cleanedCondition'";
            }
		}	
	    if($sqlcondition == "") 
		   $where = "";
		else
		   $where = " WHERE " . $sqlcondition;   		
        $query = "SELECT * from $tablename $where";


        if (!($result = @ mysqli_query ($this->connection, $query)))
         $this->showerror($query);
         
        // Find out how many rows there are
        $rowsFound = @ mysqli_num_rows($result);
                    
        if($rowsFound == 1) {        
            $row = @ mysqli_fetch_assoc($result); 
        }

        if (!empty($row)) return $row;
        else return null;           
    }
	
	// - field and condition are column and value pair
	// returns single column value
    public function getField($tablename, $field, $condition='') {
        
        $row = array();
        $sqlcondition = '';

        // set up condition sql 
        if ($condition != '') {
            if(is_array($condition)) {
                $firsttime = true;
                foreach($condition as $key => $value) {
                    if($firsttime == false)
                       $sqlcondition .= ' and ';
                   $cleanedValue = mysqli_escape_string($this->connection, $value);
                   $sqlcondition .= $key.'='."'$cleanedValue'";
                   $firsttime = false;
                }
 //               $key = array_keys($condition);
 //               $value = array_values($condition);
 //               $sqlcondition = $key[0].'='."'$value[0]'";
            }
            else {
                $keyfield = $this->_getKeyField($tablename);
                $cleanedCondition = mysqli_escape_string($this->connection, $condition);
                $sqlcondition = "$keyfield='$cleanedCondition'";
            }
		}	
	    if($sqlcondition == "") 
		   $where = "";
		else
		   $where = " WHERE " . $sqlcondition;   		
        $query = "SELECT $field from $tablename $where";
        
        if(!($result = @ mysqli_query($this->connection, $query)))
           $this->showerror($query);
           
        $rowsFound = @ mysqli_num_rows($result);
        
        if($rowsFound == 0)
           return null;
        else  {
           $row[] = @ mysqli_fetch_assoc($result);
           return $row[0][$field];
        }
    }
    public function getMaxID($tablename, $field) {
        
        $row = array();

        $query = "SELECT MAX($field) as maxvalue from $tablename";
        
        if(!($result = @ mysqli_query($this->connection, $query)))
           $this->showerror($query);
           
        $rowsFound = @ mysqli_num_rows($result);
        
        if($rowsFound == 0)
           return null;
        else  {
           $row = @ mysqli_fetch_object($result);
           return $row->maxvalue;
        }
    }

    public function setFieldByCondition($tablename, $condition, $field, $value) {
        
        $query = "SELECT $field FROM $tablename WHERE $condition";
        $result = @ mysqli_query($this->connection, $query);
        $rowsFound = @ mysqli_num_rows($result);
        if($rowsFound < 1) return null;
        else if($rowsFound > 0) {
	 	    $value = stripslashes($value);
		    $value = mysqli_real_escape_string($this->connection, $value);
            $query = "UPDATE $tablename set $field='$value' WHERE $condition";
            $result = @ mysqli_query($this->connection, $query);
            if($result == true) return true;
            else return false;
        }
    }
    
    public function setFieldByKey($tablename, $key, $field, $value) {
        
        $keyfield = $this->_getKeyField($tablename);
        $query = "SELECT $keyfield FROM $tablename WHERE $keyfield='$key'";
        $result = @ mysqli_query($this->connection, $query);
        $rowsFound = @ mysqli_num_rows($result);
        if($rowsFound < 1) return null;
        else if($rowsFound > 0) {
	  	    $value = stripslashes($value);
		    $value = mysqli_real_escape_string($this->connection, $value);
            $query = "UPDATE $tablename set $field='$value' WHERE $keyfield='$key'";
            $result = @ mysqli_query($query, $this->connection);
            if($result == true) return true;
            else return false;
        }
    }
    
    // update - if there is a $condition
    // add - if no $condition
    public function saveRow($tablename, $fieldarray, $condition='') {
        
          // set up condition sql 
          if ($condition != '') {
            if(is_array($condition)) {
                $firsttime = true;
                foreach($condition as $key => $value) {
                    if($firsttime == false)
                       $sqlcondition .= ' and ';
                   $cleanedValue = mysqli_escape_string($this->connection, $value);
                   $sqlcondition .= $key.'='."'$cleanedValue'";
                   $firsttime = false;
                }
//                $key = array_keys($condition);
//                $value = array_values($condition);
//                $sqlcondition = $key[0].'='."'$value[0]'";
            }
            else {
                $keyfield = $this->_getKeyField($tablename);
                $cleanedCondition = mysqli_escape_string($this->connection, $condition);
                $sqlcondition = "$keyfield='$cleanedCondition'";
            }
		  }	
	      if($sqlcondition == "") 
		     $where = "";
		  else
		     $where = " WHERE " . $sqlcondition;   		

            // find all columns to make sure fields in Fieldarray match the table    
            $query = "SHOW COLUMNS from $tablename";
            if(!($result = @ mysqli_query($this->connection, $query)))
               $this->showerror($query);
            $rowsFound = @ mysqli_num_rows($result);
            
            for($i=0; $i<$rowsFound; $i++) {        
                $row[] = @ mysqli_fetch_assoc($result);
                $columns[] = $row[$i]['Field']; 
            }
		
			// if have condition then its an UPDATE
            if ($condition != '' && $this->checkTableForKey($tablename, $condition)) {
                // update row
               $query = "UPDATE $tablename SET ";                   
               foreach ($fieldarray as $field=>$value) {
                   if(!is_string($field) || $field == 'id')
                        continue;  
                   if(is_array($value))
                        continue;               
				   $value = stripslashes($value);
			       $value = mysqli_real_escape_string($this->connection, $value);
                   if ((in_array($field, $columns)))  {    // anything in $fieldarray should be updated if also in 'columns' array
                      $query .= " $field='$value',";
                   }
               }
               $query = substr_replace($query,"",-1); 
               $query .= $where;                   
               if(!($result = @ mysqli_query($this->connection, $query)))
                   $this->showerror($query);
				   
            }
            else if($condition == '') { 
               // insert new row
               $query1 = "INSERT INTO $tablename (";        
               $query2 = ") VALUES (";
               foreach ($fieldarray as $field=>$value) { 
                   if(!is_string($field) || $field == 'id')
                        continue;  
                   if(is_array($value))
                        continue;               
 				   $value = stripslashes($value);
			       $value = mysqli_real_escape_string($this->connection, $value);
                   if ((in_array($field,$columns)))  {     // anything in $fieldarray should be updated if also in 'columns' array 
                       $query1 .= " $field,";
                       $query2 .= " '$value',";
                   }
               }
               $query1 = substr_replace($query1,"",-1);   
               $query2 = substr_replace($query2,"",-1);
               $query = $query1.$query2.")"; 
               if(!($result = @ mysqli_query($this->connection, $query)))
                   $this->showerror($query);
            }
            else
               return true;
            
            return true;                         
        }

    public function addRow($tablename, $fieldarray) {
        
       // insert new row
       $query1 = "INSERT INTO $tablename (";        
       $query2 = ") VALUES (";
       foreach ($fieldarray as $field=>$value) { 
           if(!is_string($field))
                continue;            
           $value = stripslashes($value);
           $value = mysqli_real_escape_string($this->connection, $value);
           $query1 .= " $field,";
           $query2 .= " '$value',";
       }
       $query1 = substr_replace($query1,"",-1);   
       $query2 = substr_replace($query2,"",-1);
       $query = $query1.$query2.")"; 
       if(!($result = @ mysqli_query($this->connection, $query)))
           $this->showerror($query);
        
       return true;                         
  }
		
	// delete row from table with key holding value of row key, or build a condition (WHERE clause)	
	public function deleteFromTable($tablename, $key, $condition='') {
		if(($key == '' || $key == false || $key == null) 
            && ($condition == '' || $condition == false || $condition == null))
		     return false;
	     // set up condition sql 
        $keyfield = $this->_getKeyField($tablename);
        if ($condition != '') {
            if(is_array($condition)) { 
                $firsttime = true;
                foreach($condition as $key => $value) {
                    if($firsttime == false)
                       $sqlcondition .= ' and ';
                   $cleanedValue = mysqli_escape_string($this->connection, $value);
                   $sqlcondition .= $key.'='."'$cleanedValue'";
                   $firsttime = false;
                }
            }
            else {
                $cleanedCondition = mysqli_escape_string($this->connection, $condition);
                $sqlcondition = "$keyfield='$cleanedCondition'";
            }
		}	
	    if($sqlcondition == "") 
		   $where = "";
		else
		   $where = " WHERE " . $sqlcondition;   	
		   
	    
		if($where == "") {
		    $keyfield = $this->_getKeyField($tablename);
            $query = "DELETE FROM $tablename WHERE $keyfield='$key'";
		}
		else {
		    $query = "DELETE FROM $tablename ". $where;
		}
	   if(!($result = @ mysqli_query($this->connection, $query)))
   			$this->showerror($query);
       else {
            // Find out how many rows were deleted
            $rowsAffected = @ mysqli_affected_rows($this->connection);  
            if($rowsAffected > 0)  
	            return true;
            else
                return false;  
       }  
	}
 
    public function checkTableForKey($tablename, $condition) {
         // set up key condition sql 
       if ($condition != '') {
            if(is_array($condition)) {
                $key = array_keys($condition);
                $value = array_values($condition);
                $sqlcondition = $key[0].'='."'$value[0]'";
            }
            else {
                $keyfield = $this->_getKeyField($tablename);
                $sqlcondition = "$keyfield='$condition'";
            }
        }
	    if($sqlcondition == "") 
		   $where = "";
		else
		   $where = " WHERE " . $sqlcondition;   		
        $query = "SELECT * from $tablename $where";
        
        if(!($result = @ mysqli_query($this->connection, $query)))
           $this->showerror($query);
           
        $rowsFound = @ mysqli_num_rows($result);
        
        if($rowsFound == 0)
          return null;
        if($rowsFound > 0)
          return true;
  }  

  // get field names for table
  function getFieldNames($tablename) {
      // find all columns to make sure fields in Fieldarray match the table    
      $query = "SHOW COLUMNS from $tablename";
      if(!($result = @ mysqli_query($this->connection, $query)))
        $this->showerror($query);
      $rowsFound = @ mysqli_num_rows($result);
    
      for($i=0; $i<$rowsFound; $i++) {        
        $row[] = @ mysqli_fetch_assoc($result);
        $columns[] = $row[$i]['Field']; 
      }
      if(!empty($columns)) 
          return $columns;
      else
          return null;
  }   
      
   
            
}   // end of class
}   // end of class_exists
    

?>