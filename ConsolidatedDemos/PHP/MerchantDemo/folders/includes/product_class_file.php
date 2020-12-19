<?php
  // products as json file
  class ProductClass {
      
      function __construct() {
      }
      
      // get all products from json file,
      //  or, apply conditional with is associative array  'field' => value, or 'field'=> array of values {'1111', '22222', '3333', ect}
      public function getProducts($condition='') {
           $jsonRows = file_get_contents('data/product_file.json');
           $rows = json_decode($jsonRows, true);    // return as associative array
           
           // process conditional selection
           if($condition != '' && is_array($condition)) {
               foreach($condition as $field => $values) {
                   if(is_array($values)) {
                       foreach($rows as $row) {        // select from array of conditional values
                           foreach($values as $value) {
                               if($row[$field] == $value) {
                                   $resultSet[] = $row;
                               }
                           }
                       }
                   }
                   else {
                       foreach($rows as $row) {
                           if($row[$field] == $values) {  // select from single conditional value
                               $resultSet[] = $row;
                           }
                       }
                   }
               }
           }
           else {
               $resultSet = $rows;
           }
               
           return $resultSet;
      }
      
  }
?>
