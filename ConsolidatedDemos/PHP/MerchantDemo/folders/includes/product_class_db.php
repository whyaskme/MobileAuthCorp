<?php
  // products as mySQL database - must also include db_class.php
  class ProductClass {
      public $DB;
      
      function __construct() {
          // 'standalone' says do blog outside of meetingfolder applciation
          $this->DB = new DbClass('localhost', 'mac_test', 'mac_user', 'mac123', 'standalone');
      }
      
      public function getProducts($condition='') {
           $rows = $this->DB->getRowsInArray('products', $condition);
           $json_rows = json_encode($rows);                           // convert to json before saving
           file_put_contents('data/product_file2.json', $json_rows);  // save for use without MySQL
           return $rows;
      }
      
      public function saveOrderInfo($fieldArray) {
          $status = $this->DB->saveRow('orders', $fieldArray);
          return $status;
      }
      
      public function saveOrderItem($fieldArray) {
          $status = $this->DB->saveRow('order_items', $fieldArray);
          return $status;
      }
      
      public function getLastInsert() {
          $id = $this->DB->doQuery('SELECT LAST_INSERT_ID()');
          return $id[0]['LAST_INSERT_ID()'];
      }
  }